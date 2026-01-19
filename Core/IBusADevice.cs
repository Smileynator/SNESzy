namespace Core;

public interface IBusADevice
{
    public void Write(uint address, byte data);
    public byte Read(uint address);
}