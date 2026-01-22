namespace Core;

[Flags]
public enum StatusRegister : byte
{
    Carry = 1,
    Zero = 1 << 1,
    InterruptOff = 1 << 2,
    DecimalMode = 1 << 3,
    IndexSize = 1 << 4,
    AccumulatorSize = 1 << 5,
    Overflow = 1 << 6,
    Negative = 1 << 7,
}

public static class StatusRegisterExtensions
{
    public static bool HasFlag(this StatusRegister value, StatusRegister flag)
    {
        return (value & flag) != 0;
    }
    
    public static void SetFlag(this ref StatusRegister value, StatusRegister flag)
    {
        value |= flag;
    }

    public static void SetFlag(this ref StatusRegister value, StatusRegister flag, bool active)
    {
        value = active 
            ? value | flag 
            : value & ~flag;
    }
    
    public static void ClearFlag(this ref StatusRegister value, StatusRegister flag)
    {
        value &= ~flag;
    }
}