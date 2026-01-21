namespace Core;

public class CPU
{
    private readonly IBusARead busRead;
    private readonly IBusAWrite busWrite;

    private ushort regA, regX, regY;
    private ushort PC, SP, directPage;
    private byte PCBank, dataBank;
    private bool emulation = true;
    private bool carry, zero, interrupt, decmode, indexSize, accSize, overflow, negative;

    private uint PCAddress => (uint)PCBank << 16 | PC;

    public CPU(IBusARead busRead, IBusAWrite busWrite)
    {
        this.busRead = busRead;
        this.busWrite = busWrite;
    }

    public void Initialize()
    {
        interrupt = true;
        indexSize = true;
        accSize = true;
        SP = 0x01FF;
        //read native mode reset vector
        PC = 0xFFFC;
        PC = ReadImmediate16();
    }

    public void Tick()
    {
        int cycles = 0;
        byte opcode = busRead.Read(PCAddress);
        PC++;

        switch (opcode)
        {
            case 0x18: //carry bit false
                carry = false;
                cycles += 2;
                break;
            case 0x78: //interrupts off
                interrupt = false;
                cycles += 2;
                break;
            case 0xFB: //exchange emulation and carry bits
                (emulation, carry) = (carry, emulation);
                cycles += 2;
                break;
        }
    }

    private ushort ReadImmediate16()
    {
        ushort result = (ushort) (busRead.Read(PCAddress) | (busRead.Read(PCAddress+1) << 8));
        PC += 2;
        return result;
    }
}