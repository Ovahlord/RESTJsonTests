namespace RESTShared
{
    public class RpcObject
    {
        public List<string> StringList = [];

        public void BloatWithUselessData(int cyclesOffset)
        {
            // This offset is used to redue the amount of data filled into the rpc object to estimate the amount of extra space needed for json to function again
            int cycles = int.MaxValue / Shared.SuperLongUselessString.Length + cyclesOffset;

            Console.WriteLine($"Filling the Rpc Object with lots of data...");
            for (int i = 0; i < cycles; ++i)
                StringList.Add(Shared.SuperLongUselessString);
            Console.WriteLine("Done.");
        }
    }
}
