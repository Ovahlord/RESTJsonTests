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
                HttpListenerContext context = await listener.GetContextAsync();
                Console.WriteLine($"Received a request message with content length of {context.Request.ContentLength64}");

                try
                {
                    using StreamReader reader = new(context.Request.InputStream, context.Request.ContentEncoding);
                    string jsonContent = await reader.ReadToEndAsync();

                    RpcObject? obj = JsonConvert.DeserializeObject<RpcObject>(jsonContent);
                    if (obj == null)
                    {
                        Console.WriteLine($"Tried to deserialize an object with a length of {jsonContent.Length} but no object as deserialized :(");
                        context.Response.Close();
                        continue;
                    }

                    Console.WriteLine($"Deserialized an object with a length of {jsonContent.Length}");
                    using StreamWriter writer = new(context.Response.OutputStream, System.Text.Encoding.UTF8);
                    await writer.WriteLineAsync($"[SERVER] Deserialized a rpc object with {obj.StringList.Count} string elements in it");
                    await writer.FlushAsync();
                    context.Response.Close(); // return the response
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    context.Response.Close();
                }
            }
        }
    }
}
