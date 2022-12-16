namespace Compiler.Constants
{
	public static class Constants
	{
		//Содержит критерии поиска для лексем типа данных и список атрибутов для него (на данный момент - идентификатор + комментарий)
		public static readonly Dictionary<string, (int, string)> Types = new Dictionary<string, (int, string)>() {
			{ "undefined",	(0, "Для неприсвоенных значений – отдельный тип, имеющий одно значение") },
            { "null",		(1, "Для неизвестных значений – отдельный тип, имеющий одно значение") },
            { "symbol",		(2, "Для уникальных идентификаторов")},
            { "boolean",	(3, "Для true/false")},
            { "number",		(4, "Для любых чисел: целочисленных или чисел с плавающей точкой") },
			{ "string",		(5, "Для строк") },
			{ "let",		(6, "Локальный тип") },
			{ "const",		(7, "Константа") }
        };

		/// <summary>
		/// return value
		/// (
		///		int		= id
		///		string	= name operation
		///		int		= priority
		///		string	= associativity (false = -> | true = <-)
		/// )
		/// </summary>
		public static readonly Dictionary<string, (int, string, int, bool)> Operators = new Dictionary<string, (int, string, int, bool)>() {
			{"=",	(0, "assign_operation", 2, true)},
			{"+=",	(1, "add_amount_operation", 2, true)},
			{"-=",	(2, "subtract_amount_operation", 2, true)},
			{"*=",	(3, "multiply_amount_operation", 2, true)},
			{"/=",	(4, "divide_amount_operation", 2, true)},
			{"%=",	(5, "modulo_amount_operation", 2, true)},
			{"<<=", (6, "left_shift_amount_operation", 2, true)},
			{">>=", (7, "right_shift_amount_operation", 2, true)},
			{"+",	(8, "sum_operation", 12, false)},
			{"-",	(9, "subtract_operation", 12, false)},
			{"*",	(10, "multiply_operation", 13, false)},
			{"/",	(11, "divide_operation", 13, false)},
			{"==",	(12, "are_equal_operation", 9, false)},
			{">",	(13, "more_operation", 10, false)},
			{"<",	(14, "less_operation", 10, false)},
			{"++",	(15, "increment_operation", 16, false)},
			{"--",	(16, "decrement_operation", 16, false)},
			{"%",	(17, "modulo_operation", 13, false)},
		};
		public static readonly string[] Keywords = { "class", "for", "return", "if", "else", "while" };

		public static readonly string[] KeySymbols = { ".", ";", ",", "(", ")", "[", "]", "{", "}", ":" };
	}
}

