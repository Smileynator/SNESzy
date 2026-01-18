namespace Core;

public class CPU
{
    private readonly IBusARead busRead;
    private readonly IBusAWrite busWrite;

    private ushort regA, regX, regY;
    private ushort PC, SP, directPage;
    private byte PCBank, dataBank;
    private bool emulation = true;
    private bool carry, zero, irq, decmode, brk, overflow, negative;

    public CPU(IBusARead busRead, IBusAWrite busWrite)
    {
        this.busRead = busRead;
        this.busWrite = busWrite;

        //read native mode reset vector
        PC = (ushort)(busRead.Read(0x00FFFC) << 8 | busRead.Read(0x00FFFD));
    }

    public void Tick()
    {
        byte opcode = busRead.Read((uint) PCBank << 16 | PC);
    }
}