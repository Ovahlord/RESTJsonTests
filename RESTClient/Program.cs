using Newtonsoft.Json;
using RESTShared;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RESTClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("REST Client ready for testing. Please push a button to continue...");

            while (true)
            {
                try
                {
                    Console.ReadLine();

                    HttpClient client = new()
                    {
                        BaseAddress = new($"{Shared.BaseUrl}")
                    };

                    // We want to fill the Rpc Object with so many data that it will be > 2gb in size to test if the Json-Serialization
                    // throws up when we push beyond the string limit
                    int cyclesOffset = -4000000; // An offset to estimate when the string reaches its memory limit.

                    // At about -4000000 the Json string reaches a functional state and can be used again. Otherwise we will get out of memory exceptions due to going beyond string limits during the deserialization
                    int cycles = int.MaxValue / Shared.SuperLongUselessString.Length + cyclesOffset;

                    Console.WriteLine($"Filling the Rpc Object with lots of data...");
                    RpcObject rpcObj = new();
                    for (int i = 0; i < cycles; ++i)
                        rpcObj.StringList.Add(Shared.SuperLongUselessString);

                    Console.WriteLine("Done. Begin serializing the string...");

                    HttpContent content = new StringContent(JsonConvert.SerializeObject(rpcObj), System.Text.Encoding.UTF8, "application/json");

                    Task<HttpResponseMessage> response = client.PostAsync(Shared.RpcTestUrl, content);

                    Console.WriteLine("Sent message. Awaiting response....");
                    response.Wait();
                    Console.WriteLine("Received response. Reading content...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
