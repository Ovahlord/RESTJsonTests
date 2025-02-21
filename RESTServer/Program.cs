using System.Net;
using RESTShared;
using Newtonsoft.Json;

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
            HttpListener listener = new()
            {
                Prefixes = { $"{Shared.BaseUrl}{Shared.RpcTestUrl}" }
            };

            listener.Start();
            Console.WriteLine($"REST Server is now listening for requests on {Shared.BaseUrl}{Shared.RpcTestUrl}...");

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

                    Console.WriteLine($"Deserialized an object with a length of {jsonContent.Length} :)");
                    context.Response.Close(); // and we're done here
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
        }
    }
}
