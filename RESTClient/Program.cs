using Newtonsoft.Json;
using RESTShared;

namespace RESTClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("REST Client ready for testing. Please push a button to continue...");
            Console.ReadLine();

            Console.WriteLine("Beginning tests...");

            // Testing the 2gb string limit when serializing lots of data into json
            testJsonSerialization(asString: true);
            // and now again by using streams instead
            testJsonSerialization(asString: false);

            Console.WriteLine("All tests have concluded");
            Thread.Sleep(5000);
        }

        private async static void testJsonSerialization(bool asString)
        {
            HttpClient client = new()
            {
                BaseAddress = new($"{Shared.BaseUrl}")
            };

            RpcObject rpcObj = new();
            rpcObj.BloatWithUselessData(0);

            if (asString)
            {
                Console.WriteLine("[TEST] Serializing a 2gb object into json as string");

                try
                {
                    string jsonContent = JsonConvert.SerializeObject(rpcObj);
                    HttpContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Shared.RpcTestUrl, content);

                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(System.OutOfMemoryException))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("An out of memory exception orrured.");
                        Console.ResetColor();
                    }
                    else
                        Console.WriteLine(ex);
                    return;
                }
            }
            else
            {
                Console.WriteLine("[TEST] Serializing a 2gb object into json as stream");

                try
                {
                    JsonSerializer serializer = new();

                    using MemoryStream stream = new();
                    using StreamWriter writer = new(stream);
                    using JsonTextWriter jsonWriter = new(writer);

                    serializer.Serialize(jsonWriter, rpcObj);
                    await stream.FlushAsync();

                    // In .net we can also go for JsonContent.Create to accomplish this.

                    HttpContent content = new StreamContent(stream);
                    HttpResponseMessage response = await client.PostAsync(Shared.RpcTestUrl, content);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(System.OutOfMemoryException))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("An out of memory exception orrured.");
                        Console.ResetColor();
                    }
                    else
                        Console.WriteLine(ex);
                    return;
                }
            }
        }
    }
}
