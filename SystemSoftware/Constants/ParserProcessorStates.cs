namespace Compiler.Constants
{
    public enum ParserProcessorStates : int
    {
        Error = -1,
        Idle = 0,
        CreateNode = 1,
        StatusOperation = 2,
        AddNode = 3,
        Final = 4
    }
}
