using System.Reflection;

namespace MyApp;

internal class Program
{
    static void Main(string[] args)
    {
        Core.Motherboard snes = new Core.Motherboard(GetResourceRomData());
        while (true) ;
    }
    
    

    private static byte[] GetResourceRomData()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream("Console.Resources.ChronoTrigger.SMC");
        if (stream == null)
            throw new FileNotFoundException();
        using MemoryStream memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        byte[] data = memoryStream.ToArray();
        //Skip headered ROM header if present.
        if (data.Length % 1024 == 512)
        {
            Span<byte> span = data.AsSpan(512);
            data = span.ToArray();
        }
        return data;
    }
}