namespace Core;

public class IOPlaceholder : IBusADevice
{
    public void BusAWrite(uint address, byte data)
    {
    }

    public byte BusARead(uint address)
    {
        return 0;
    }
}