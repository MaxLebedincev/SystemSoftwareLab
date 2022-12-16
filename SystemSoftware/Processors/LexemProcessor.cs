using System;
using System.Text.RegularExpressions;
using Compiler.Constants;
using Compiler.Models;

namespace Compiler.Processors
{
    public class LexemProcessor
    {
        private string _buffer = "";
        private string _seekingBuffer = "";
        private int _pointer = 0;
        private LexemProcessorStates _state = LexemProcessorStates.Idle;
        private StringReader? _reader;
        private char[] _charBuffer = new char[1];
        private List<Lexem> _lexemsView = new List<Lexem>();
        private List<Variable> _variablesTable = new List<Variable>();
        private int _variablesCounter = 0;

        public LexemProcessor()
        {

        }

        public Tuple<IList<Lexem>, IList<Variable>> ProcessFile(string fileName)
        {
            using (_reader = new StringReader(File.ReadAllText(fileName)))
            {
                while (_state != LexemProcessorStates.Final)
                {
                    switch (_state)
                    {
                        case LexemProcessorStates.Idle:
                            {
                                if (_reader.Peek() == -1)
                                {
                                    _state = LexemProcessorStates.Final;
                                    break;
                                }

                                if (IsEmptyOrNextLine(_charBuffer[0]))
                                {
                                    GetNextChar();
                                }
                                else if (char.IsLetter(_charBuffer[0]))
                                {
                                    ClearBuffer();
                                    AddToBuffer(_charBuffer[0]);
                                    _state = LexemProcessorStates.ReadingIdentifier;
                                    GetNextChar();
                                }
                                else if (char.IsDigit(_charBuffer[0]))
                                {
                                    ClearBuffer();
                                    AddToBuffer(_charBuffer[0]);
                                    _state = LexemProcessorStates.ReadingNum;
                                    GetNextChar();
                                }
                                else
                                {
                                    _state = LexemProcessorStates.Delimeter;
                                    AddToBuffer(_charBuffer[0]);
                                    GetNextChar();
                                }
                                break;
                            }
                        case LexemProcessorStates.ReadingIdentifier:
                            {
                                if (char.IsLetterOrDigit(_charBuffer[0]))
                                {
                                    AddToBuffer(_charBuffer[0]);
                                    GetNextChar();
                                }
                                else
                                {
                                    var lexemRef = SearchInLexemDictionary();
                                    var typeRef = SearchInTypesDictionary();
                                    var booleanType = _buffer == "true" || _buffer == "false" ? true : false;
                                    var undefinedType = _buffer == "undefined" ? true : false;
                                    var nullType = _buffer == "null" ? true : false;
                                    if (lexemRef.Item1 != -1)
                                    {
                                        AddLexem(LexemTypes.Identifier, lexemRef.Item1, lexemRef.Item2, _buffer);
                                        ClearBuffer();
                                    }
                                    else if (typeRef.Item1 != -1)
                                    {
                                        if (_buffer == "let" || _buffer == "const")
                                        {
                                            AddLexem(LexemTypes.DataType, typeRef.Item1, typeRef.Item2, _buffer);
                                        }
                                        else
                                        {
                                            //_variablesTable.Add(new Variable(_variablesCounter++, _buffer, _bufferLexem.Value));

                                            _lexemsView[_lexemsView.Count - 2].Value = _buffer;
                                            _variablesTable[_variablesTable.Count - 1].DataType = _buffer;

                                            AddLexem(LexemTypes.DataType, typeRef.Item1, typeRef.Item2, _buffer);
                                        }
                                        ClearBuffer();
                                    }
                                    else if (booleanType)
                                    {
                                        AddLexem(LexemTypes.Constant, 0, getConstantDataType(_buffer), _buffer);
                                        ClearBuffer();
                                    }
                                    else if (undefinedType)
                                    {
                                        AddLexem(LexemTypes.Constant, 0, getConstantDataType(_buffer), _buffer);
                                        ClearBuffer();
                                    }
                                    else if (nullType)
                                    {
                                        AddLexem(LexemTypes.Constant, 0, getConstantDataType(_buffer), _buffer);
                                        ClearBuffer();
                                    }
                                    else
                                    {
                                        var variable = _variablesTable.Any(v => v.Name.Equals(_buffer));
                                        if (!variable)
                                        {
                                            if (_lexemsView.Last().Type != LexemTypes.DataType)
                                            {
                                                _state = LexemProcessorStates.Error;
                                                break;
                                            }

                                            _variablesTable.Add(new Variable(_variablesCounter++, "error", _buffer));
                                            AddLexem(LexemTypes.Variable, _variablesTable.Count - 1, "error", _buffer);
                                            ClearBuffer();
                                        }
                                        else
                                        {

                                            AddLexem(LexemTypes.Variable, _variablesTable.FindIndex(c => c.Name == _buffer), _variablesTable[_variablesTable.FindIndex(c => c.Name == _buffer)].DataType, _buffer);
                                            ClearBuffer();
                                        }
                                    }
                                    _state = LexemProcessorStates.Idle;
                                }
                                break;
                            }
                        case LexemProcessorStates.ReadingNum:
                            {
                                if (char.IsDigit(_charBuffer[0]))
                                {
                                    AddToBuffer(_charBuffer[0]);
                                    GetNextChar();
                                }
                                else
                                {
                                    AddLexem(LexemTypes.Constant, int.Parse(_buffer), getConstantDataType(_buffer), _buffer);
                                    ClearBuffer();
                                    _state = LexemProcessorStates.Idle;
                                }
                                break;
                            }
                        case LexemProcessorStates.Delimeter:
                            {
                                var searchResult = SearchInDelimeterDictionary();
                                var searchOperatorsResult = SearchInOperationsDictionary();


                                if (searchResult.Item1 != -1)
                                {
                                    AddLexem(LexemTypes.Delimeter, searchResult.Item1, searchResult.Item2, searchResult.Item2);
                                    _state = LexemProcessorStates.Idle;
                                    ClearBuffer();
                                }
                                else if (searchOperatorsResult.Item1 != -1)
                                {
                                    _seekingBuffer = char.ToString(_buffer[0]);
                                    while (IsOperator(_charBuffer[0]))
                                    {
                                        _seekingBuffer += _charBuffer[0];
                                        GetNextChar();
                                    }
                                    var seekOperatorsResult = SeekInOperationsDictionary();
                                    if (seekOperatorsResult.Item1 != -1)
                                    {
                                        AddLexem(LexemTypes.Operation, seekOperatorsResult.Item1, seekOperatorsResult.Item2, _seekingBuffer);
                                        _state = LexemProcessorStates.Idle;
                                        ClearBuffer();
                                    }
                                    else
                                    {
                                        AddLexem(LexemTypes.ParsingError, -1, $"Error at {_pointer}: Could not parse {_buffer}!", _buffer);
                                        _state = LexemProcessorStates.Error;
                                    }
                                }
                                else
                                {
                                    AddLexem(LexemTypes.ParsingError, -1, $"Error at {_pointer}: Could not parse {_buffer}!", _buffer);
                                    _state = LexemProcessorStates.Error;
                                }
                                break;
                            }
                        case LexemProcessorStates.Error:
                            {
                                _state = LexemProcessorStates.Final;
                                break;
                            }
                        case LexemProcessorStates.Final:
                            {
                                return new Tuple<IList<Lexem>, IList<Variable>>(_lexemsView, _variablesTable);
                            }
                    }
                }

                return new Tuple<IList<Lexem>, IList<Variable>>(_lexemsView, _variablesTable);
            }
        }

        private void GetNextChar()
        {
            _reader.Read(_charBuffer, 0, 1);
            _pointer++;
        }

        private char PeekNextChar()
        {
            return (char)_reader.Peek();
        }

        private bool IsEmptyOrNextLine(char input)
        {
            return input == ' '
                || input == '\n'
                || input == '\t'
                || input == '\0'
                || input == '\r';
        }

        private bool IsOperator(char input)
        {
            return input == '='
                || input == '+'
                || input == '-'
                || input == '/'
                || input == '*'
                || input == '%'
                || input == '>'
                || input == '<'
                || input == '%'
                || input == '^'
                || input == '|';
        }

        private void ClearBuffer()
        {
            _buffer = "";
            _seekingBuffer = "";
        }

        private void AddToBuffer(char input)
        {
            _buffer += input;
        }

        private void AddLexem(LexemTypes type, int value, string lex, string realLex)
        {
            _lexemsView.Add(new Lexem(type, value, lex, realLex));
        }

        private (int, string) SearchInLexemDictionary()
        {
            var result = Array.FindIndex(Constants.Constants.Keywords, l => l.Equals(_buffer));

            if (result != -1)
            {
                return (result, _buffer);
            }

            return (-1, _buffer);
        }

        private (int, string) SearchInDelimeterDictionary()
        {
            var result = Array.FindIndex(Constants.Constants.KeySymbols, l => l.Equals(_buffer));

            if (result != -1)
            {
                return (result, _buffer);
            }

            return (-1, _buffer);
        }

        private (int, string) SearchInTypesDictionary()
        {
            var searchResult = Constants.Constants.Types.ContainsKey(_buffer);

            if (searchResult)
            {
                return Constants.Constants.Types[_buffer];
            }

            return (-1, _buffer);
        }

        private (int, string, int, bool) SearchInOperationsDictionary()
        {
            var searchResult = Constants.Constants.Operators.ContainsKey(_buffer);

            if (searchResult)
            {
                return Constants.Constants.Operators[_buffer];
            }

            return (-1, _buffer, -1, false);
        }

        private (int, string, int, bool) SeekInOperationsDictionary()
        {
            var searchResult = Constants.Constants.Operators.ContainsKey(_seekingBuffer);

            if (searchResult)
            {
                return Constants.Constants.Operators[_seekingBuffer];
            }

            return (-1, _buffer, -1, false);
        }

        private string getConstantDataType(string value)
        {
            if (Regex.IsMatch(value, "[0-9]+"))
            {
                return "number";
            }
            else if (value == "true" || value == "false")
            {
                return "boolean";
            }
            else if (Regex.IsMatch(value, "\'.\'"))
            {
                return "symbol";
            }
            else if (Regex.IsMatch(value, "\".+\""))
            {
                return "string";

            }
            else if (value == "null")
            {
                return "null";
            }
            else if (value == "undefined")
            {
                return "undefined";
            }
            else
            {
                return "error";
            }
        }
    }
}

