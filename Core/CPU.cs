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

    private uint PCAddress => (uint)PCBank << 16 | PC;

    public CPU(IBusARead busRead, IBusAWrite busWrite)
    {
        this.busRead = busRead;
        this.busWrite = busWrite;
    }

    public void Initialize()
    {
        //read native mode reset vector
        PC = (ushort)(busRead.Read(0x00FFFC) << 8 | busRead.Read(0x00FFFD));
    }

    public void Tick()
    {
        byte opcode = busRead.Read(PCAddress);
    }
}