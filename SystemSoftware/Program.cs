using Compiler.Models;
using Compiler.Processors;
using Compiler.Utils;
using Newtonsoft.Json;

FileManager file = new FileManager("Example2");

LexemProcessor processor = new LexemProcessor();
var result = processor.ProcessFile(file.GetCurrentFileFromCode());

Console.WriteLine("Lexems: ");
Console.WriteLine("================================================");
foreach (var lexem in result.Item1)
{
	Console.WriteLine(lexem.ToString());
}
Console.WriteLine("================================================");
Console.WriteLine("Variables: ");
foreach (var variable in result.Item2)
{
	Console.WriteLine(variable.ToString());
}
Console.WriteLine("================================================");

List<Operation>? parser = new SyntacticProcessor(result).getSyntaxTree();

#region Запись Синтаксического дерева
string s = JsonConvert.SerializeObject(parser);
File.WriteAllText(file.GetCurrentFileFromSyntaxTree(), s);
#endregion

List<Operation>? semanticProcessor = new SemanticProcessor(parser, result).GetSemanticTree();

#region Запись Семантического дерева
s = JsonConvert.SerializeObject(semanticProcessor);
File.WriteAllText(file.GetCurrentFileFromSemanticTree(), s);
#endregion

var IntermediateCode = new GeneratorIntermediateCode(result, parser, semanticProcessor);

Console.WriteLine($"Промежуточный код: \n");
Console.WriteLine(IntermediateCode.GetAllText());
Console.WriteLine($"Количество символов: {IntermediateCode.CountSymbols()}");

#region Запись Промежуточного кода
File.WriteAllText(file.GetCurrentFileFromIntermediateCode(), IntermediateCode.GetAllText());
#endregion