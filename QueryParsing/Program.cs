using System.Management.Automation;
using System.Management.Automation.Language;

namespace QueryParsing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var script = ScriptBlock.Create("'michael' -eq 'Michael'");

            Token[] tokens;
            ParseError[] errors;
            var ast = Parser.ParseInput(script.ToString(), out tokens, out errors);
            var instrument = new InstrumentAst<Person>();
            var output = instrument.VisitElement(ast);
        }
    }
}