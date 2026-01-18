using System.Text;

namespace Core;

internal class CartridgeHeader
{
    public string Title { get; }
    public RomMap RomMap { get; }
    public Speed RomSpeed { get; }
    public Capabilities Capabilities { get; }
    public Coprocessor Coprocessor { get; }
    public int RomSizeBytes { get; }
    public int RamSizeBytes { get; }
    public AreaCode AreaCode { get; }
    public byte DeveloperId { get; }
    public byte Revision { get; }
    public ushort ChecksumCompliment { get; }
    public ushort Checksum { get; }

    public CartridgeHeader(ReadOnlySpan<byte> headerData, RomMap expectedMap)
    {
        if (headerData.Length != 32)
            throw new EndOfStreamException(nameof(headerData));
        
        Title = Encoding.ASCII.GetString(headerData.Slice(0, 21));
        
        ChecksumCompliment = (ushort) (headerData[28] << 8 | headerData[29]);
        Checksum = (ushort) (headerData[30] << 8 | headerData[31]);
        if((Checksum ^ ChecksumCompliment) != 0xFFFF)
            throw new ArithmeticException($"Checksum and Compliment do not match {Checksum.ToString("X4")}/{ChecksumCompliment.ToString("X4")}.");

        int romMap = headerData[21] & 0b0000_0111;
        switch (romMap)
        {
           case 0:
           case 3://SA-1 causes this for some reason
               RomMap = RomMap.LoROM;
               break;
           case 1:
               RomMap = RomMap.HiROM;
               break;
           case 5:
               RomMap = RomMap.ExHiROM;
               break;
        }
        if(!Enum.IsDefined(RomMap))
            throw new ArgumentOutOfRangeException(nameof(headerData), $"Rom Map is invalid {RomMap}.");
        //8 roms, of which at least 4 legit ones seem to fail this check.
        //if(RomMap != expectedMap)
        //    throw new ArgumentOutOfRangeException(nameof(headerData), $"Rom Map actual {expectedMap} does not match header location {RomMap} - {romMap.ToString("X2")}.");
        RomSpeed = (Speed)(headerData[21] & 0b0001_0000);
        
        Capabilities = (Capabilities)(headerData[22] & 0b0000_1111);
        if(!Enum.IsDefined(Capabilities))
            throw new ArgumentOutOfRangeException(nameof(headerData), $"Capabilities value is invalid {Capabilities}.");
        Coprocessor = Coprocessor.None;
        if (Capabilities >= Capabilities.ROM_CoProc)
            Coprocessor = (Coprocessor)((headerData[22] & 0b1111_0000) >> 4);
        if(!Enum.IsDefined(Coprocessor))
            throw new ArgumentOutOfRangeException(nameof(headerData), $"Coprocessor value is invalid {Coprocessor}.");

        RomSizeBytes = (1 << headerData[23]) * 1024;
        RamSizeBytes = (1 << headerData[24]) * 1024;
        
        AreaCode = (AreaCode) headerData[25];
        if(!Enum.IsDefined(AreaCode))
            throw new ArgumentOutOfRangeException(nameof(headerData), $"AreaCode value is invalid {AreaCode}.");
        
        DeveloperId = headerData[26];
        Revision = headerData[27];
    }

    public override string ToString()
    {
        return base.ToString() + " - " + Title;
    }
}

internal enum RomMap
{
    LoROM,
    HiROM,
    ExHiROM,
}

internal enum Speed
{
    Slow = 0,
    Fast = 1,
}

internal enum Capabilities
{
    ROM = 0,
    ROM_RAM = 1,
    ROM_RAM_Battery = 2,
    ROM_CoProc = 3,
    ROM_CoProc_RAM = 4,
    ROM_CoProc_RAM_Battery = 5,
    ROM_CoProc_Battery = 6,
    ROM_CoProc_RAM_StarFox2 = 10,
}

internal enum Coprocessor
{
    None = -1,
    DSP = 0,
    GSU_SuperFX = 1,
    OBC1 = 2,
    SA1 = 3,
    SDD1 = 4,
    SRTC = 5,
    Other = 14,
    Custom = 15,
}

internal enum AreaCode
{
    Japan = 0,
    NorthAmerica = 1,
    Europe = 2,
    Scandinavia = 3,
    French = 6,
    Dutch = 7,
    Spanish = 8,
    German = 9,
    Italian = 10,
    Chinese = 11,
    Korean = 13,
    Common = 14,
    Canadian = 15,
    Brazilian = 16,
    Australian = 17,
    OtherX = 18,
    OtherY = 19,
    OtherZ = 20,
    KrustySuperFunHouse = 81,
}