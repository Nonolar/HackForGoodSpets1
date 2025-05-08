using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Security.Principal;
using System.Diagnostics;

namespace HTTPHandlerer;

class HTTPUtils : HTTPDecoder
{
    //WHY THE FUCK YELLOW, YOU LITERALLY CAN'T USE THIS CLASS WITHOUT INITIATING THIS VARIABLE WHY THE FUCK DOES IT GET YELLOW?
    //AND I WON'T USE REQUIRED BECAUSE IT IS MORE TROUBLE THAN IT'S WORTH.
    static HttpListener Listener;
    static ConcurrentQueue<HttpListenerContext> asyncQue;

    public HTTPUtils(string IP = "127.0.0.1:80"){
        Listener = new();
        Listener.Prefixes.Add($"http://{IP}/");
        asyncQue = new();
        _ = StartServer();
    }

    public async Task StartServer(){
        if (!IsAdministrator()) {
            RestartAsAdmin();
            return;
        }

        if (!TryStartListener()) {
            return;
        }

        await ListenLoop();
    }

    private bool TryStartListener() {
        try {
            Listener.Start();
            return true;
        }
        catch (HttpListenerException ex) {
            if (ex.ErrorCode == 5) { // Access Denied
                Console.WriteLine("Failed to start server - Access Denied");
                RestartAsAdmin();
                return false;
            }
            else if (ex.ErrorCode == 32) { // Port already in use
                Console.WriteLine("Port is already in use. Please ensure port is available.");
                Environment.Exit(1);
            }
            else {
                Console.WriteLine($"Failed to start server: {ex.Message}");
                Environment.Exit(1);
            }
            return false;
        }
    }

    private static bool IsAdministrator() {
        if (OperatingSystem.IsWindows()) {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        Console.WriteLine("Administrator check is only supported on Windows.");
        return false;
    }

    private string UnableToFindFileName(){
        Console.WriteLine("Unable to find filename of active process\nTerminating program");
        Environment.Exit(1);
        return "Error";
    }
    private void RestartAsAdmin() {
        if (IsAdministrator()) {
            Console.WriteLine("Already running with administrator privileges.");
            return;
        }

        var startInfo = new ProcessStartInfo {
            UseShellExecute = true,
            FileName = Process.GetCurrentProcess().MainModule?.FileName ?? UnableToFindFileName(),
            Verb = "runas"
        };

        try {
            Process.Start(startInfo);
            Environment.Exit(0);
        }
        catch (System.ComponentModel.Win32Exception) {
            Console.WriteLine("The server needs administrator privileges to bind to port 80.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
    async Task ListenLoop(){
        Console.WriteLine("Trying to start");
        try{
            Listener.Start();
        }catch(Exception ex){
            Console.WriteLine(ex.Message);
        }
        while(true){
            HttpListenerContext context = await Listener.GetContextAsync();

            HttpListenerRequest Request = context.Request;
            HttpListenerResponse Response = context.Response;

            if(Request.HttpMethod == "OPTIONS"){
                Response.StatusCode = 200;
                Response.Headers.Add("Allow", "GET");
                Response.Close();
            }

            //Let's not for the moment implement a POST. Options should only return GET
            if(Request.HttpMethod == "GET"){
                //Add the thing to a ASYNCQue
                asyncQue.Append(context);
                Response.Close();
            }
        }
    }

    

    public async Task<string?> GetLatestRequest()
    {
        if(asyncQue.IsEmpty) return null;
        HttpListenerContext? ToReturn;
        
        asyncQue.TryPeek(out ToReturn);
        Console.WriteLine(ToReturn.Request.ToString());
        if(ToReturn != null) return ToReturn.Request.ToString();
        return null;
    }


    public async Task<bool> SendResponse(byte[] bytes)
    {
        throw new NotImplementedException();
    }

    public byte[] JsonToByte(string Json)
    {
        throw new NotImplementedException();
    }
}