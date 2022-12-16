namespace Compiler.Models
{
	public class Variable
	{
		public int Id { get;  set; }
		public string? DataType { get; set; }
		public string? Name { get;  set; }

		public Variable(int _id, string _dataType, string _varName)
		{
			Id = _id;
			DataType = _dataType;
			Name = _varName;
		}

		public Variable()
        {

        }

		public override string ToString()
		{
			return $"<{Id}> Variable of type <{DataType}> with name <{Name}>";
		}
	}
}

