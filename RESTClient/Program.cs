using Newtonsoft.Json;
using RESTShared;

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

                    // We will create an object full of junk data to serialize to push the json serialization in the next step to its limits
                    RpcObject rpcObj = new();
                    rpcObj.BloatWithUselessData();

                    // Serialize the rpc object and initialize the content for the upcoming POST message
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(rpcObj), System.Text.Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> response = client.PostAsync(Shared.RpcTestUrl, content);

                    Console.WriteLine($"[CLIENT] Sent message with a serialized rpc object with {rpcObj.StringList.Count} string elements ...");
                    response.Wait();

                    // Response has been received. Reads its content, which contains information about the data that has been deserialized
                    using StreamReader reader = new(response.Result.Content.ReadAsStream());
                    Console.WriteLine(reader.ReadToEnd());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
