using System.Net;

interface HTTPDecoder{
    //public event EventHandler GotHTTP;
    public Task<string?> GetLatestRequest();
    public Task<bool> SendResponse(byte[] bytes);
    public byte[] JsonToByte(string Json);
}

public interface ITokensStruct;
interface ITokens{
    
    public ITokensStruct[] Parse(string JSON);
}
interface IParser{
    public enum Stmt;
    public  Stmt[] Parse(ITokensStruct Tokens);
}


interface IGenerator{
    public string Generate(Enum[] Stmnts);
}