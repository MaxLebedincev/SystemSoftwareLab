namespace Compiler.Constants
{
	public enum LexemProcessorStates : int
	{
		Idle = 1,
		ReadingNum = 2,
		Delimeter = 3,
		Completed = 4,
		ReadingIdentifier = 5,
		Assign = 6,
		Error = 7,
		Final = 8,
		ScopeOpened = 9,
		ScopeClosed = 10,
	}
}

