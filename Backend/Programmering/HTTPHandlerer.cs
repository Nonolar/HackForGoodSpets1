using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Security.Principal;
using System.Diagnostics;
using System.Text;

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
        
        while(true){
            HttpListenerContext context = await Listener.GetContextAsync();

            HttpListenerRequest Request = context.Request;
            HttpListenerResponse Response = context.Response;
            Console.WriteLine(Request.HttpMethod);
            if(Request.HttpMethod == "OPTIONS"){
                Response.StatusCode = 200;
                Response.Headers.Add("Allow", "POST");
                Response.Headers.Add("Access-Control-Allow-Origin", "*");
                Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
                Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                Response.Close();
                continue;
            }

            if(Request.HttpMethod == "POST"){
                Console.WriteLine("Posting to queue");
                try{
                    asyncQue.Enqueue(context); // Use Enqueue instead of Append
                }catch(Exception ex){
                    Console.WriteLine($"Failed to enqueue request: {ex.Message}");
                    Response.StatusCode = 500;
                    Response.Close();
                }
                // Don't close the response here as we'll handle it in SendResponse
            } else {
                // For any other HTTP method
                Response.StatusCode = 405; // Method Not Allowed
                Response.Headers.Add("Allow", "POST, OPTIONS");
                Response.Close();
            }
        }
    }

    

    public async Task<string?> GetLatestRequest()
    {
        //Console.WriteLine("Kör");
        if(asyncQue.IsEmpty) return null;
        HttpListenerContext? ToReturn;
        Console.WriteLine("Kör");
        
        asyncQue.TryPeek(out ToReturn);
        Console.WriteLine(ToReturn.Request.ToString());
        using (var reader = new StreamReader(ToReturn.Request.InputStream, ToReturn.Request.ContentEncoding))
        {
            string jsonContent = await reader.ReadToEndAsync();
            return jsonContent;
        }
        return null;
    }


    public async Task<bool> SendResponse(byte[] bytes)
    {
        Console.WriteLine("Sending bytes");
        HttpListenerContext context ;
        asyncQue.TryDequeue(out context);
        if(context == null) return false;
        HttpListenerResponse response = context.Response;
        try{
            response.StatusCode = 200;
            System.IO.Stream output = response.OutputStream;
            Console.WriteLine("Starting broadcast");
            await output.WriteAsync(bytes, 0, bytes.Length);
            output.Close();
            response.Close();
            Console.WriteLine("Output closed");
            return true;
        }catch(Exception ex){
            Console.WriteLine($"Uncaught error: {ex.Message}");
            return false;
        }
    }

    public byte[] JsonToByte(string Json)
    {
        return Encoding.UTF8.GetBytes(Json);
    }
}