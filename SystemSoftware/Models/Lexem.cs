using Compiler.Constants;

namespace Compiler.Models
{
	public class Lexem
	{
		public LexemTypes Type { get; private set; }
		public int Lex { get; private set; }
		public string Value { get; set; }
		public string RealValue { get; private set; }

		/// <summary>
        /// Creates a new lexem object
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_lex"></param>
        /// <param name="_val"></param>
        public Lexem(LexemTypes _type, int _lex, string _val, string _realValue)
        {
            Type = _type;
            Lex = _lex;
            Value = _val;
            RealValue = _realValue;
        }

        public override string ToString()
		{
			return $"lexem type: {Type};\t lexem id: {Lex};\t value: {Value}";
		}
	}
}

