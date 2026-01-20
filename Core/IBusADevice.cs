namespace Core;

public interface IBusADevice
{
    public void BusAWrite(uint address, byte data);
    public byte BusARead(uint address);
}