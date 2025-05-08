namespace Tokenizer;

class TokenHandlerer : ITokens{
    public struct Tokens{
        public string name;
        public dynamic value;
    }
    public ITokens.Tokens[] Parse(string JSON){
        List<ITokens.Tokens> ParsedTokens = new();

        // Example logic to populate ParsedTokens
        // ParsedTokens.Add(new Tokens { name = "example", value = "exampleValue" });

        return ParsedTokens.ToArray();
    }

    static Dictionary<string, DataJSON> ParseJson(string JSON){
        string[] Lines = JSON.Split('\n');
        Dictionary<string, DataJSON> keyValuePairs = new();


        return keyValuePairs;
    }

    struct DataJSON{
        public string type;
        public List<string> nextNodes;
        public List<string> prevNodes;
        public string identification;
        public string value;
    }
}