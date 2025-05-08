using System.Net;
using System.Threading.Tasks;
using HTTPHandlerer;
using Tokenizer;

namespace Programmering;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        HTTPUtils hTTP = new("*:80");
        ITokens tokens = new TokenHandlerer();
        IParser parser; //Non implemented yet
        IGenerator generator; //Non implemented Yet
        while(true){
            string OldestJson = await hTTP.GetLatestRequest();
            if(OldestJson == null) continue;
            ITokens.Tokens[] ParsedTOkens = tokens.Parse(OldestJson); 
            //IParser.Stmt[] Stmts = parser.Parse(ParsedTOkens);
            //string code = generator.Generate(ParsedTOkens);
        }
        Console.ReadKey();
        
    }
}
