namespace Core;

public class WorkRam : IBusADevice
{
    private byte[] wram = new byte[0x20000];
    
    public void BusAWrite(uint address, byte data)
    {
        if ((address & 0xFE0000) == 0x7E0000)//full region
        {
            wram[address & 0x1FFFF] = data;
        }
        else if ((address & 0x40E000) == 0)//low ram repeats
        {
            wram[address & 0x1FFF] = data;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(address));
        }
    }

    public byte BusARead(uint address)
    {
        if ((address & 0xFE0000) == 0x7E0000)//full region
        {
            return wram[address & 0x1FFFF];
        }
        else if ((address & 0x40E000) == 0)//low ram repeats
        {
            return wram[address & 0x1FFF];
        }
        throw new ArgumentOutOfRangeException(nameof(address));
    }
}