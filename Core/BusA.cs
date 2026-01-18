namespace Core;

public class BusA : IBusARead, IBusAWrite
{
    private byte openBus;
    private readonly Func<uint,byte>[] readTable;
    private readonly Action<uint,byte>[] writeTable;

    public BusA()
    {
        readTable = new Func<uint,byte>[0xFFFFFF];
        writeTable = new Action<uint,byte>[0xFFFFFF];
    }

    public byte Read(uint address)
    {
        return 0;//readTable[address].Invoke(address);
    }

    public void Write(uint address, byte data)
    {
        //writeTable[address].Invoke(address, data);
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