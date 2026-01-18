namespace Core;

public class Cartridge
{
    private readonly CartridgeHeader header;
    
    public Cartridge(byte[] romData)
    {
        List<Exception> exceptions = new List<Exception>();
        Span<byte> span = romData.AsSpan();
        
        CartridgeHeader? foundHeader = TryHeader(span, 0x007FC0, RomMap.LoROM, exceptions) ??
                                        TryHeader(span, 0x00FFC0, RomMap.HiROM, exceptions) ??
                                        TryHeader(span, 0x40FFC0, RomMap.ExHiROM, exceptions);
        
        if(foundHeader == null)
            throw new AggregateException("Rom header can not be found.", exceptions);
        header = foundHeader;
    }

    private static CartridgeHeader? TryHeader(Span<byte> span, int address, RomMap expectedMap, List<Exception> exceptions)
    {
        if (address >= span.Length)
            return null;
        try
        {
            return new CartridgeHeader(span.Slice(address, 32), expectedMap);
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
            return null;
        }
    }
}