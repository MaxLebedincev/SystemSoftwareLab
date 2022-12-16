using Compiler.Constants;
using Compiler.Models;

namespace Compiler.Processors
{
    public class SyntacticProcessor
    {
        private List<Operation> SyntaxTree = new List<Operation>();

        private Operation node = null;
        private int counterId = -1;
        private List<Operation> internalNode = new List<Operation>();
        private ParserProcessorStates processorState = ParserProcessorStates.Idle;
        public SyntacticProcessor(Tuple<IList<Lexem>, IList<Variable>> Lexems)
        {
            foreach (var lexem in Lexems.Item1)
            {
                processorState = ParserProcessorStates.Idle;

                while (processorState != ParserProcessorStates.Final)
                {
                    switch (processorState)
                    {
                        case ParserProcessorStates.Idle:

                            if (lexem.Type == LexemTypes.Variable || lexem.Type == LexemTypes.Constant)
                            {
                                processorState = ParserProcessorStates.CreateNode;
                            }
                            else if (lexem.Type == LexemTypes.Operation)
                            {
                                processorState = ParserProcessorStates.StatusOperation;
                            }
                            else if (lexem.Type == LexemTypes.Delimeter)
                            {
                                processorState = ParserProcessorStates.AddNode;
                            }
                            else if (lexem.Type == LexemTypes.DataType || lexem.Type == LexemTypes.Identifier)
                            {
                                processorState = ParserProcessorStates.Final;
                            }
                            else
                            {
                                processorState = ParserProcessorStates.Error;
                            }
                            break;

                        case ParserProcessorStates.CreateNode:

                            if (node == null)
                            {
                                node = new Operation()
                                {
                                    Id = 0,
                                    value = null,
                                    variable = new List<Variable>()
                                    {
                                        new Variable
                                        (
                                            ++counterId,
                                            lexem.Value,
                                            lexem.RealValue
                                        )
                                    },
                                    childNode = null
                                };
                            }
                            else if (CheckArgOperation(node.value, node.variable.Count))
                            {
                                node.variable.Add(new Variable(
                                    ++counterId,
                                    lexem.Value,
                                    lexem.RealValue
                                ));
                            }

                            processorState = ParserProcessorStates.Final;

                            break;

                        case ParserProcessorStates.StatusOperation:

                            if (node.value == null)
                            {
                                node.value = lexem.RealValue;
                                node.Id = ++counterId;
                            }
                            else
                            {
                                internalNode.Add(node);
                                node = new Operation()
                                {
                                    Id = ++counterId,
                                    value = lexem.RealValue,
                                    variable = new List<Variable>(),
                                    childNode = null
                                };
                            }

                            if (node.value == "++" || node.value == "--")
                            {
                                node.variable.Add(new Variable(
                                    ++counterId,
                                    "number",
                                    node.value == "++" ? "1" : "-1"
                                ));
                            }

                            processorState = ParserProcessorStates.Final;

                            break;

                        case ParserProcessorStates.AddNode:

                            if (lexem.Value == ";" || lexem.Value == ")")
                            {
                                internalNode.Add(node);

                                for (int i = internalNode.Count - 1; i > 0; i--)
                                {
                                    internalNode[i - 1].childNode = internalNode[i];
                                }

                                SyntaxTree.Add(internalNode[0]);

                                internalNode = new List<Operation>();
                                node = null;
                            }

                            processorState = ParserProcessorStates.Final;

                            break;
                    }
                }
            }

        }

        private bool CheckArgOperation(string? operationName, int countNode)
        {
            bool operationFlag = false;
            int operationNumber = -1;

            foreach (var elem in Constants.Constants.Operators)
            {
                operationFlag = elem.Key == operationName ? true : operationFlag;
                operationNumber = elem.Key == operationName ? elem.Value.Item1 : operationNumber;
            }

            if (operationFlag)
            {
                return countNode < 2;
            }
            else
            {
                return false;
            }
        }

        public List<Operation> getSyntaxTree()
        {
            return SyntaxTree;
        }
    }
}
