using Compiler.Constants;
using Compiler.Models;
using System.Text;

namespace Compiler.Processors
{
    public class GeneratorIntermediateCode
    {
        private int currentNumberNode = 0;
        private int tabCount = 0;
        private string generateCode = "";
        private int allSymbol = 0;

        private GeneratorIntermediateStates processorState = GeneratorIntermediateStates.Idle;
        public GeneratorIntermediateCode(Tuple<IList<Lexem>, IList<Variable>> Lexems, List<Operation>? SyntaxTree, List<Operation>? SemanticTree)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                foreach (var lexem in Lexems.Item1)
                {
                    processorState = GeneratorIntermediateStates.Idle;

                    while (processorState != GeneratorIntermediateStates.Final)
                    {
                        switch (processorState)
                        {
                            case GeneratorIntermediateStates.Idle:

                                if (lexem.Type == LexemTypes.DataType)
                                {
                                    processorState = GeneratorIntermediateStates.DataType;
                                }
                                else if (lexem.Type == LexemTypes.Delimeter)
                                {
                                    processorState = GeneratorIntermediateStates.Delimeter;
                                }
                                else if (lexem.Type == LexemTypes.Identifier)
                                {
                                    sb.Append(" " + lexem.RealValue);
                                    processorState = GeneratorIntermediateStates.Final;
                                }
                                else if (lexem.Type == LexemTypes.Operation || lexem.Type == LexemTypes.Variable || lexem.Type == LexemTypes.Constant)
                                {
                                    processorState = GeneratorIntermediateStates.Operation;
                                }
                                else
                                {
                                    processorState = GeneratorIntermediateStates.Error;
                                }

                                break;

                            case GeneratorIntermediateStates.Delimeter:

                                if (lexem.RealValue == ";")
                                {
                                    sb.Append(lexem.RealValue);
                                }
                                else if (lexem.RealValue != ":")
                                {
                                    sb.Append(" " + lexem.RealValue);
                                }

                                if (lexem.RealValue == ";" || lexem.RealValue == ")")
                                {
                                    ++currentNumberNode;
                                }

                                if (lexem.RealValue == ";" || lexem.RealValue == "{" || lexem.RealValue == "}")
                                {
                                    sb.Append($"\n");
                                }

                                tabCount += lexem.RealValue == "{" ? 1 : lexem.RealValue == "}" ? -1 : 0;

                                processorState = GeneratorIntermediateStates.Final;

                                break;

                            case GeneratorIntermediateStates.DataType:

                                if (lexem.RealValue == "let" || lexem.RealValue == "const")
                                {
                                    sb.Append(" " + lexem.RealValue);
                                }

                                processorState = GeneratorIntermediateStates.Final;

                                break;

                            case GeneratorIntermediateStates.Operation:

                                #region Контроль узлов нод
                                var elemSyntax = SyntaxTree[currentNumberNode];
                                var elemSemantic = SemanticTree[currentNumberNode];
                                #endregion

                                sb.Append(" " + lexem.RealValue);

                                processorState = GeneratorIntermediateStates.Final;

                                break;

                            case GeneratorIntermediateStates.Error:

                                processorState = GeneratorIntermediateStates.Final;
                                throw new Exception();

                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Ошибка при генерации кода!");
            }

            sb.AppendLine();
            generateCode = sb.ToString();
            allSymbol = sb.Length;
        }

        private string Tab()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(" ");

            for (int i = 0; i < tabCount; i++)
            {
                stringBuilder.Append("   ");
            }

            return stringBuilder.ToString();
        }

        public string GetAllText()
        {
            return generateCode;
        }

        public int CountSymbols()
        {
            return allSymbol;
        }
    }
}
