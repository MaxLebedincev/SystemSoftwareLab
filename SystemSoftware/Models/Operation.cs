namespace Compiler.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public string? value { get; set; }
        public List<Variable>? variable { get; set; }
        public Operation? childNode { get; set; }
    }
}
