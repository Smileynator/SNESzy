namespace Core;




public partial class CPU
{
    private readonly IBusARead busRead;
    private readonly IBusAWrite busWrite;

    private ushort regA, regX, regY;
    private ushort PC, SP, directPage;
    private byte PCBank, dataBank;
    private bool emulation = true;
    private StatusRegister P;

   

    private uint PCAddress => (uint)PCBank << 16 | PC;

    public CPU(IBusARead busRead, IBusAWrite busWrite)
    {
        this.busRead = busRead;
        this.busWrite = busWrite;
    }

    public void Initialize()
    {
        P.SetFlag(StatusRegister.InterruptOff | StatusRegister.IndexSize | StatusRegister.AccumulatorSize);
        SP = 0x01FF;
        //read native mode reset vector
        PC = 0xFFFC;
        PC = ReadImmediate16();
    }

    public void Tick()
    {
        int cycles = 0;
        byte opcode = ReadImmediate8();

        cycles += ExecuteOpcode(opcode);
    }
    
    private byte ReadImmediate8()
    {
        byte result = busRead.Read(PCAddress);
        PC++;
        return result;
    }

    private ushort ReadImmediate16()
    {
        ushort result = (ushort) (busRead.Read(PCAddress) | (busRead.Read(PCAddress+1) << 8));
        PC += 2;
        return result;
    }
    
    private uint ReadImmediate24()
    {
        uint result = (uint) (busRead.Read(PCAddress) | (busRead.Read(PCAddress+1) << 8) | (busRead.Read(PCAddress+2) << 16));
        PC += 3;
        return result;
    }
}