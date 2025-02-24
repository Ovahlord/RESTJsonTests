using Newtonsoft.Json;
using RESTShared;
using System.Net;


namespace RESTServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task listen = ListenToRequests();
            listen.Wait();
            Console.WriteLine("RESTServer has been terminated.");
        }

        public static async Task ListenToRequests()
        {
            // Setting up the endpoint for the server.
            HttpListener listener = new()
            {
                Prefixes = { $"{Shared.BaseUrl}{Shared.RpcTestUrl}" }
            };

            listener.Start();
            Console.WriteLine($"REST Server is now listening for requests on {Shared.BaseUrl}...");

            // Listening to requests. We will continously keep listening to requests which we accomplish with this while loop
            while (true)
            {
                try
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    RpcObject? rpcObj = null;
                    HttpListenerRequest request = context.Request;
                    Console.WriteLine($"Content length = {request.ContentLength64}");

                    if (request.ContentType == "application/json")
                    {
                        Console.WriteLine("Content type is Json");

                        using StreamReader reader = new(request.InputStream, request.ContentEncoding);
                        string jsonContent = await reader.ReadToEndAsync();

                        rpcObj = JsonConvert.DeserializeObject<RpcObject>(jsonContent);
                    }
                    else
                    {
                        JsonSerializer serializer = new();
                        using MemoryStream stream = new();
                        await request.InputStream.CopyToAsync(stream);
                        stream.Position = 0;

                        using StreamReader reader = new(stream, System.Text.Encoding.UTF8);
                        using JsonTextReader jsonReader = new(reader);
                        rpcObj = serializer.Deserialize<RpcObject>(jsonReader);
                    }

                    if (rpcObj == null)
                    {
                        Console.WriteLine($"Could not deserialize rpc object provided by the request");
                        context.Response.Close();
                        continue;
                    }

                    //using StreamWriter writer = new(context.Response.OutputStream, System.Text.Encoding.UTF8);
                    //await writer.WriteLineAsync($"[SERVER] Deserialized a rpc object with {rpcObj.StringList.Count} string elements in it");
                    //await writer.FlushAsync();
                    context.Response.Close(); // return the response
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    using FileStream fs = new("crashlog.txt", FileMode.Append);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(ex);
                }
            }
        }
    }
}
