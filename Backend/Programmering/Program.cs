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
            //Console.WriteLine("Getting Json");
            string OldestJson = await hTTP.GetLatestRequest();
            //Console.WriteLine(OldestJson);
            if(OldestJson == null) continue;
            Console.WriteLine(OldestJson);
            Tokens[] ParsedTOkens = tokens.Parse(OldestJson).Select(t => (Tokens)t).ToArray(); 
            //IParser.Stmt[] Stmts = parser.Parse(ParsedTOkens);
            //string code = generator.Generate(ParsedTOkens);

            await hTTP.SendResponse(hTTP.JsonToByte("Test"));
        }
        Console.ReadKey();
        
    }
}
