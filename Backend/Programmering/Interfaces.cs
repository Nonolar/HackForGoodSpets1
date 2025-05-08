using System.Net;

interface HTTPDecoder{
    //public event EventHandler GotHTTP;
    public Task<string?> GetLatestRequest();
    public Task<bool> SendResponse(byte[] bytes);
    public byte[] JsonToByte(string Json);
}

interface ITokens{
    public struct Tokens;
    public ITokens.Tokens[] Parse(string JSON);
}
interface IParser{
    public enum Stmt;
    public  Stmt[] Parse(ITokens.Tokens[] Tokens);
}


interface IGenerator{
    public string Generate(Enum[] Stmnts);
}