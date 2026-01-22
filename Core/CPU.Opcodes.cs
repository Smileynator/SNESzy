namespace Core;

public partial class CPU
{
    private int ExecuteOpcode(byte opcode)
    {
        int cycles = 0;
        switch (opcode)
        {
            case 0x18:
                P.ClearFlag(StatusRegister.Carry);
                cycles += 2;
                break;
            case 0x38:
                P.SetFlag(StatusRegister.Carry);
                cycles += 2;
                break;
            case 0x5C:
                ushort newPC = ReadImmediate16();
                PCBank = ReadImmediate8();
                PC = newPC;
                cycles += 4;
                break;
            case 0x58:
                P.ClearFlag(StatusRegister.InterruptOff);
                cycles += 2;
                break;
            case 0x78:
                P.SetFlag(StatusRegister.InterruptOff);
                cycles += 2;
                break;
            case 0xA2:
                regX = ReadImmediate16();
                P.SetFlag(StatusRegister.Negative, (regX & 0x8000) != 0);
                bool isZero = regX == 0;
                P.SetFlag(StatusRegister.Zero, isZero);
                cycles += isZero ? 3 : 2;
                break;
            case 0xB8:
                P.ClearFlag(StatusRegister.Overflow);
                cycles += 2;
                break;
            case 0xC2:
                P.ClearFlag((StatusRegister) ReadImmediate8());
                cycles += 3;
                break;
            case 0xE2:
                P.SetFlag((StatusRegister) ReadImmediate8());
                cycles += 3;
                break;
            case 0xD8:
                P.ClearFlag(StatusRegister.DecimalMode);
                cycles += 2;
                break;
            case 0xF8:
                P.SetFlag(StatusRegister.DecimalMode);
                cycles += 2;
                break;
            case 0xFB: //exchange emulation and carry bits
                bool temp = emulation;
                emulation = P.HasFlag(StatusRegister.Carry);
                if (temp)
                    P.SetFlag(StatusRegister.Carry);
                else
                    P.ClearFlag(StatusRegister.Carry);
                cycles += 2;
                break;

            #region Transfer between registers

            case 0xAA:
            {
                bool halfIndex = P.HasFlag(StatusRegister.IndexSize);
                if (halfIndex)
                {
                    byte transfer = (byte)regA;
                    regX = (ushort)((regX & 0xFF00) | transfer);
                    P.SetFlag(StatusRegister.Negative, (transfer & 0x80) != 0);
                    P.SetFlag(StatusRegister.Zero, transfer == 0);
                }
                else
                {
                    regX = regA;
                    P.SetFlag(StatusRegister.Negative, (regA & 0x8000) != 0);
                    P.SetFlag(StatusRegister.Zero, regA == 0);
                }
                cycles += 2;
                break;
            }
            case 0xA8:
            {
                bool halfIndex = P.HasFlag(StatusRegister.IndexSize);
                if (halfIndex)
                {
                    byte transfer = (byte)regA;
                    regY = (ushort)((regY & 0xFF00) | transfer);
                    P.SetFlag(StatusRegister.Negative, (transfer & 0x80) != 0);
                    P.SetFlag(StatusRegister.Zero, transfer == 0);
                }
                else
                {
                    regY = regA;
                    P.SetFlag(StatusRegister.Negative, (regA & 0x8000) != 0);
                    P.SetFlag(StatusRegister.Zero, regA == 0);
                }
                cycles += 2;
                break;
            }
            case 0x5B:
            {
                directPage = regA;
                P.SetFlag(StatusRegister.Negative, (regA & 0x8000) != 0);
                P.SetFlag(StatusRegister.Zero, regA == 0);
                cycles += 2;
                break;
            }
            case 0x1B:
            {
                SP = regA;
                P.SetFlag(StatusRegister.Negative, (regA & 0x8000) != 0);
                P.SetFlag(StatusRegister.Zero, regA == 0);
                cycles += 2;
                break;
            }
            case 0x7B:
            {
                regA = directPage;
                P.SetFlag(StatusRegister.Negative, (directPage & 0x8000) != 0);
                P.SetFlag(StatusRegister.Zero, directPage == 0);
                cycles += 2;
                break;
            }
            case 0x3B:
            {
                regA = SP;
                P.SetFlag(StatusRegister.Negative, (SP & 0x8000) != 0);
                P.SetFlag(StatusRegister.Zero, SP == 0);
                cycles += 2;
                break;
            }
            #endregion
            
            default:
                throw new NotImplementedException("Unknown opcode " + opcode.ToString("X2"));
        }

        return cycles;
    }
}