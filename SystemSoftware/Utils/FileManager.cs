namespace Compiler.Utils
{
    public class FileManager
    {
        private static string _PathRoot = AppDomain.CurrentDomain.BaseDirectory + @"../../../";
        private const string pfExample = @"Examples/";
        private const string pfCode = @"Code/";
        private const string pfSyntaxTree = @"SyntaxTree/";
        private const string pfSemanticTree = @"SemanticTree/";
        private const string pfIntermediateCode = @"IntermediateCode/";

        public string CurrentFileName = @"Example1";
        public string CurrentExpand = @"txt";

        public FileManager(string fileName, string expand = "txt")
        {
            CurrentFileName = fileName;
            CurrentExpand = expand;
        }
        public FileManager(){}
        public static string GetRoot()
        {
            return _PathRoot;
        }
        public string GetCurrentFileFromCode()
        {
            return _PathRoot + pfExample + pfCode + CurrentFileName + ".txt";
        }
        public string GetCurrentFileFromSyntaxTree()
        {
            return _PathRoot + pfExample + pfSyntaxTree + CurrentFileName + ".json";
        }
        public string GetCurrentFileFromSemanticTree()
        {
            return _PathRoot + pfExample + pfSemanticTree + CurrentFileName + ".json";
        }

        public string GetCurrentFileFromIntermediateCode()
        {
            return _PathRoot + pfExample + pfIntermediateCode + CurrentFileName + ".txt";
        }
    }
}
