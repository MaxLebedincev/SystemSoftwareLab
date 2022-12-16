using Compiler.Models;
using System.Text.RegularExpressions;

namespace Compiler.Processors
{
    public class SemanticProcessor
    {
        private List<NodeOperation> CurrentListOperation = new List<NodeOperation>();
        private List<NodeVariable> CurrentListVariable = new List<NodeVariable>();
        private List<NodeOperation> PriorityQueueOperation = new List<NodeOperation>();
        private NodeVariable bufferVariableFirst = null;
        private NodeVariable bufferVariableSecond = null;
        private NodeVariable bufferMainVariable = null;
        private string bufferType = null;
        private bool IsOutputVariable = false;
        private bool IsComparison = false;

        private List<Operation> internalNode = new List<Operation>();
        private List<Operation> SemanticTree = new List<Operation>();

        public SemanticProcessor(List<Operation> syntaxTree, Tuple<IList<Lexem>, IList<Variable>> Lexems)
        {
            foreach (Operation MainNode in syntaxTree)
            {
                try
                {
                    if (MainNode != null || MainNode.value != null)
                    {
                        AnalysisNode(MainNode);
                        CreatePriorityQueue(CurrentListOperation);

                        if (PriorityQueueOperation[0] != null)
                            IsMainVariable(CurrentListVariable[0], PriorityQueueOperation[0]);

                        foreach (NodeOperation currentOperation in PriorityQueueOperation)
                        {
                            foreach (NodeVariable currentVariable in CurrentListVariable)
                            {
                                if (currentOperation.Id - 1 == currentVariable.Id)
                                {
                                    bufferVariableFirst = currentVariable;
                                }
                                else if (currentOperation.Id + 1 == currentVariable.Id)
                                {
                                    bufferVariableSecond = currentVariable;
                                }
                            }

                            UpdateTypeByOperation(currentOperation);
                            bufferType = null;
                        }

                        internalNode.Reverse();

                        for (int i = internalNode.Count - 1; i > 0; i--)
                        {
                            internalNode[i - 1].childNode = internalNode[i];
                        }

                        SemanticTree.Add(internalNode[0]);

                        ClearBuffer();
                    }
                }
                catch (Exception)
                {
                    ClearBuffer();
                }
            }
        }

        private void AnalysisNode(Operation node)
        {
            while (node != null)
            {
                CurrentListOperation.Add(new NodeOperation()
                {
                    Id = node.Id,
                    Name = node.value,
                    isCheck = false,
                    Associativity = false
                });

                if (node.value == "==" || node.value == ">" || node.value == "<")
                {
                    IsComparison = true;
                }

                foreach (Variable? variable in node.variable)
                {
                    CurrentListVariable.Add(new NodeVariable()
                    {
                        Id = variable.Id,
                        Name = variable.Name,
                        DataType = variable.DataType,
                        isCheck = false
                    });
                }

                node = node.childNode;
            }
        }

        private void CreatePriorityQueue(List<NodeOperation> operations)
        {
            for (int i = 0; i < operations.Count; i++)
            {
                (int, string, int, bool) curreantValue = (0, null, 0, false);
                NodeOperation currentOperation = operations[i];
                int currentId = i;

                for (int j = i; j < operations.Count; j++)
                {
                    var answer = Constants.Constants.Operators.ContainsKey(operations[j].Name);

                    if (answer)
                    {
                        if (curreantValue.Item3 < Constants.Constants.Operators[operations[j].Name].Item3)
                        {
                            curreantValue = Constants.Constants.Operators[operations[j].Name];
                            currentOperation = operations[j];
                            currentId = j;
                        }

                    }
                }

                operations[currentId] = operations[i];
                operations[i] = currentOperation;

                PriorityQueueOperation.Add(new NodeOperation()
                {
                    Id = currentOperation.Id,
                    Name = currentOperation.Name,
                    isCheck = false,
                    Associativity = curreantValue.Item4
                });
            }
        }
        private void IsMainVariable(NodeVariable variable, NodeOperation operation)
        {
            bool answer = Constants.Constants.Operators.ContainsKey(operation.Name);

            if (answer)
            {
                IsOutputVariable = !Regex.IsMatch(variable.Name, "[0-9]+")
                    && variable.Name != "false"
                    && variable.Name != "true";

                if (!IsOutputVariable)
                {
                    IsOutputVariable =
                        Constants.Constants.Operators[operation.Name].Item1 < 8
                        && Constants.Constants.Operators[operation.Name].Item1 > -1;

                    if (IsOutputVariable)
                    {
                        Console.WriteLine($"Ошибка при семантическом анализе! В значение с Id = {variable.Id}! Невозможно присвоить значение константе!");
                        throw new Exception();
                    }
                }
                else
                {
                    IsOutputVariable =
                        Constants.Constants.Operators[operation.Name].Item1 < 8
                        && Constants.Constants.Operators[operation.Name].Item1 > -1;
                }

                if (IsOutputVariable) bufferMainVariable = variable;
            }

        }

        private void UpdateTypeByOperation(NodeOperation operation)
        {
            if (Constants.Constants.Operators[PriorityQueueOperation.Last().Name].Item3 == 2)
            {
                switch (Constants.Constants.Operators[operation.Name].Item3)
                {
                    case 2:

                        if (Constants.Constants.Operators[operation.Name].Item1 == 0)
                        {
                            if (bufferType != null)
                            {
                                bufferVariableFirst.DataType = bufferType;
                            }
                            else
                            {
                                bufferVariableFirst.DataType = bufferVariableSecond.DataType;
                            }
                        }
                        else
                        {
                            bufferVariableFirst.DataType = "number";
                        }

                        break;
                    case 12:

                        bufferType = "number";

                        break;
                    case 13:

                        bufferType = "number";

                        break;
                    case 16:

                        if (bufferVariableFirst.Name == "true" || bufferVariableFirst.Name == "null" || bufferVariableFirst.Name == "false")
                        {
                            Console.WriteLine($"Ошибка при семантическом анализе! В значение с Id = {bufferVariableFirst.Id}! Невозможно выполнить операцию инкрементации!");
                            throw new Exception();
                        }

                        bufferType = "number";

                        break;
                }
            }

            if (PriorityQueueOperation.Last().Id == operation.Id)
            {
                internalNode.Add(new Operation()
                {
                    Id = operation.Id,
                    value = operation.Name,
                    variable = new List<Variable>()
                    {
                        new Variable()
                        {
                            Id = bufferVariableFirst.Id,
                            DataType = bufferVariableFirst.DataType,
                            Name = bufferVariableFirst.Name
                        },
                        new Variable()
                        {
                            Id = bufferVariableSecond.Id,
                            DataType = bufferVariableSecond.DataType,
                            Name = bufferVariableSecond.Name
                        }
                    }
                });
            }
            else
            {
                if (IsComparison)
                {
                    int id = bufferVariableSecond.Id;
                    bufferVariableSecond.Id = bufferVariableFirst.Id;

                    for (int i = 0; i < CurrentListVariable.Count; i++)
                    {
                        if (CurrentListVariable[i].Id == id)
                        {
                            CurrentListVariable[i].Id = bufferVariableFirst.Id;
                        }
                        else if (CurrentListVariable[i].Id == bufferVariableFirst.Id)
                        {
                            CurrentListVariable[i].Id = id;
                            bufferVariableFirst.Id = id;
                        }
                    }
                }

                internalNode.Add(new Operation()
                {
                    Id = operation.Id,
                    value = operation.Name,
                    variable = new List<Variable>()
                    {
                        new Variable()
                        {
                            Id = bufferVariableSecond.Id,
                            DataType = bufferVariableSecond.DataType,
                            Name = bufferVariableSecond.Name
                        }
                    }
                });
            }
        }
        private void ClearBuffer()
        {
            CurrentListOperation = new List<NodeOperation>();
            CurrentListVariable = new List<NodeVariable>();
            PriorityQueueOperation = new List<NodeOperation>();
            bufferVariableFirst = null;
            IsOutputVariable = false;
            IsComparison = false;
            bufferMainVariable = null;
            bufferVariableSecond = null;
            bufferType = null;
            internalNode = new List<Operation>();
        }

        public List<Operation> GetSemanticTree()
        {
            return SemanticTree;
        }
    }
}
