namespace Core;

public class BusA : IBusARead, IBusAWrite
{
    private static readonly ushort regionSize = 0x2000;
    private IBusADevice[] regions = new IBusADevice[0x1000000/regionSize];
    private byte openBus;

    public BusA()
    {
        
    }
    
    public void RegisterBusRegion(byte bankFrom, byte bankTo, ushort addressFrom, ushort addressTo, IBusADevice device)
    {
        if (bankFrom > bankTo)
            throw new ArgumentException("Param order wrong", nameof(bankFrom));
        if (addressFrom > addressTo)
            throw new ArgumentException("Param order wrong", nameof(addressFrom));
        if (addressFrom % regionSize != 0)
            throw new ArgumentException("Address not dividable by region",  nameof(addressFrom));
        if (addressTo % regionSize != 0x1FFF)
            throw new ArgumentException("Address end of dividable region",  nameof(addressTo));
        
        uint from = (uint) addressFrom >> 13;
        uint to = (uint) addressTo >> 13;
        for (uint bank = bankFrom; bank <= bankTo; bank++)
        {
            uint bankOffset = bank << 3;
            for (uint address = from; address <= to; address++)
            {
                uint region = bankOffset | address;
                if (regions[region] != null)
                    throw new AccessViolationException($"Region {(region << 13).ToString("X6")} already defined!");
                regions[region] = device;
            }
        }
    }

    public byte Read(uint address)
    {
        return regions[address >> 13].BusARead(address);
    }

    public void Write(uint address, byte data)
    {
        regions[address >> 13].BusAWrite(address, data);
    }
}


public interface IBusAWrite
{
    public void Write(uint address, byte data);
}

public interface IBusARead
{
    public byte Read(uint address);
}