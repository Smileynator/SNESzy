using System.IO.Compression;
using Shouldly;
using Xunit;

namespace Core.Test;

public class InitializationTests
{
    [Fact]
    public void CanParseAllHeaders()
    {
        //Arrange
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string resourceDir = Path.Combine(projectDirectory, "Resources", "Roms");

        Dictionary<string, Exception?> exceptions = new Dictionary<string, Exception?>();
        foreach (string zipPath in Directory.EnumerateFiles(resourceDir, "*.zip"))
        {
            using ZipArchive archive = ZipFile.OpenRead(zipPath);
            try
            {
                ZipArchiveEntry entry = archive.Entries.Single();
                byte[] data = GetRomDataFromZipEntry(entry);
                //Act
                Exception? exception = Record.Exception(() => new Cartridge(data));
                exceptions.Add(Path.GetFileNameWithoutExtension(zipPath), exception);
            }
            catch (Exception ex)
            {
                exceptions.Add(Path.GetFileNameWithoutExtension(zipPath), ex);
            }
        }
        
        //Assert
        KeyValuePair<string, Exception?>[] failed = exceptions.Where(pair => pair.Value != null).ToArray();
        failed.ShouldBeEmpty("Roms failed to initialize:\n" + string.Join("\n", failed.Select(p => p.Key)));
    }
    
    private static byte[] GetRomDataFromZipEntry(ZipArchiveEntry entry)
    {
        using Stream stream = entry.Open();
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