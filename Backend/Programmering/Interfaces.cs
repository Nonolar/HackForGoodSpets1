using System.Net;

interface HTTPDecoder{
    //public event EventHandler GotHTTP;
    public string GetLatestRequest();
    public bool SendResponse(byte[] bytes);
    public byte[] JsonToByte(string Json);
}

interface Tokens{
    public enum Tokens;
    public Tokens[] Parse(string JSON);
}
interface IParser{
    public enum Stmt;
    public  Stmt[] Parse(Enum[] Tokens);
}


interface Generator{
    public string Generate(Enum[] Stmnts);
}