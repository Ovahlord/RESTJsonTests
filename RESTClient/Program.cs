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

                    RpcObject rpcObj = new();

                    {
                        // Artificially bloating the rpc object with lots of strings to test the length limit of json strings.

                        // This offset is used to redue the amount of data filled into the rpc object to estimate the amount of extra space needed for json to function again
                        int cyclesOffset = -4000000;
                        int cycles = int.MaxValue / Shared.SuperLongUselessString.Length + cyclesOffset;

                        Console.WriteLine($"Filling the Rpc Object with lots of data...");
                        for (int i = 0; i < cycles; ++i)
                            rpcObj.StringList.Add(Shared.SuperLongUselessString);
                    }

                    Console.WriteLine("Done. Begin serializing the string...");

                    // Serialize the rpc object and initialize the content for the upcoming POST message
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(rpcObj), System.Text.Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> response = client.PostAsync(Shared.RpcTestUrl, content);


                    Console.WriteLine("Sent message. Awaiting response....");
                    response.Wait();

                    // Response has been received. Reads its content, which contains information about the data that has been deserialized
                    Console.WriteLine("Received response. Reading content...");
                    using StreamReader reader = new(response.Result.Content.ReadAsStream());
                    string responseString = reader.ReadToEnd();
                    Console.WriteLine($"Response content: {responseString}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
