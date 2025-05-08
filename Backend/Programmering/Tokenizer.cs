using System.Text.Json;

namespace Tokenizer;
public struct Tokens : ITokensStruct {
        public string name { get; set; }
        public dynamic value { get; set; }
    }
class TokenHandlerer : ITokens{
    
    public ITokensStruct[] Parse(string JSON){
        List<Tokens> ParsedTokens = new();

        // Example logic to populate ParsedTokens
        // ParsedTokens.Add(new Tokens { name = "example", value = "exampleValue" });
        Dictionary<string, DataJSON> ParsedJSON = ParseJson(JSON);

        
        DataJSON currentNode = ParsedJSON.Values.ToList().Find(n => n.prevNodes == null);
        List<DataJSON> Nodes = [currentNode];
        while(Nodes.Count < ParsedJSON.Values.Count){
            //Don't count with more than one node after current one
            currentNode = ParsedJSON[currentNode.nextNodes[0]];
            Nodes.Add(currentNode);
        }
        foreach(var Node in Nodes){
            ParsedTokens.Add(new Tokens
            {
                name = Node.type,
                value = Node.value
            });
        }
        return ParsedTokens.Cast<ITokensStruct>().ToArray();
    }

    static Dictionary<string, DataJSON>? ParseJson(string JSON){
        JsonDocument document = JsonDocument.Parse(JSON);

        Dictionary<string, DataJSON> keyValuePairs = new();

        foreach (var element in document.RootElement.EnumerateObject())
        {
            DataJSON data = new DataJSON
            {
                type = element.Value.GetProperty("type").GetString(),
                nextNodes = element.Value.GetProperty("nextNodes").EnumerateArray().Select(e => e.GetString()).ToList(),
                prevNodes = element.Value.GetProperty("prevNodes").EnumerateArray().Select(e => e.GetString()).ToList(),
                identification = element.Value.GetProperty("identification").GetString(),
                value = element.Value.GetProperty("value").GetString()
            };

            try{
                keyValuePairs.Add(data.identification, data);
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        return keyValuePairs;
    }

    struct DataJSON{
        public string? type;
        public List<string>? nextNodes;
        public List<string>? prevNodes;
        public string? identification;
        public string? value;
    }
}