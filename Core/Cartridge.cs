namespace Core;

public class Cartridge : IBusADevice
{
    private readonly CartridgeHeader header;
    private readonly byte[] romData;
    
    public Cartridge(byte[] romData)
    {
        this.romData = romData;
        header = GetCartridgeHeader(romData);
        
    }

    private static CartridgeHeader GetCartridgeHeader(byte[] romData)
    {
        List<Exception> exceptions = new List<Exception>();
        Span<byte> span = romData.AsSpan();
        
        CartridgeHeader? foundHeader = TryHeader(span, 0x007FC0, RomMap.LoROM, exceptions) ??
                                       TryHeader(span, 0x00FFC0, RomMap.HiROM, exceptions) ??
                                       TryHeader(span, 0x40FFC0, RomMap.ExHiROM, exceptions);
        
        if(foundHeader == null)
            throw new AggregateException("Rom header can not be found.", exceptions);
        return foundHeader;
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

    public byte BusARead(uint address)
    {
        switch (header.RomMap)
        {
            case RomMap.LoROM:
            {
                throw new NotImplementedException();
                break;
            }
            case RomMap.HiROM:
            {
                if ((address & 0x3FFFFF) > header.RomSizeBytes)
                    return 0; //open bus i guess
                if (address > 0xC00000) //straight map
                    return romData[address & 0x3FFFFF];
                if ((address & 0x408000) == 0x8000) //upper half and "lower" banks, upper half mapped
                    return romData[address & 0xFFFF];
                return 0; //open bus i guess
            }
            case RomMap.ExHiROM:
            {
                throw new NotImplementedException();
                break;
            }
        }
        throw new NotImplementedException();
    }

    public void BusAWrite(uint address, byte data)
    {
        //we ignore it, ROM is ROM until SRAM
    }
}