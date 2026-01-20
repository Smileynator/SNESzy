namespace Core;

public class Motherboard
{
    //PAL clock speed 21.28137MHz
    private Cartridge cartridge;
    private BusA busA;
    private CPU cpu;
    private WorkRam wram;
    private IOPlaceholder placeholder;

    public Motherboard(byte[] romData)
    {
        wram = new WorkRam();
        cartridge = new Cartridge(romData);
        busA = new BusA();
        cpu = new CPU(busA, busA);
        placeholder = new IOPlaceholder();
        
        MapBusRegions();

        for (int i = 0; i < 100; i++)
        {
            cpu.Tick();
        }
    }

    private void MapBusRegions()
    {
        //ROM mirror
        busA.RegisterBusRegion(0x00, 0x3F, 0x8000, 0xFFFF, cartridge);
        busA.RegisterBusRegion(0x40, 0x7D, 0x0000, 0xFFFF, cartridge);
        //ROM
        busA.RegisterBusRegion(0x80, 0xBF, 0x8000, 0xFFFF, cartridge);
        busA.RegisterBusRegion(0xC0, 0xFF, 0x0000, 0xFFFF, cartridge);
        //Cart Expansion
        busA.RegisterBusRegion(0x00, 0x3F, 0x6000, 0x7FFF, cartridge);
        busA.RegisterBusRegion(0x80, 0xBF, 0x6000, 0x7FFF, cartridge);
        //WRAM
        busA.RegisterBusRegion(0x7E, 0x7F, 0x0000, 0xFFFF, wram);
        busA.RegisterBusRegion(0x00, 0x3F, 0x0000, 0x1FFF, wram);
        busA.RegisterBusRegion(0x80, 0xBF, 0x0000, 0x1FFF, wram);
        //IO Placeholder
        busA.RegisterBusRegion(0x00, 0x3F, 0x2000, 0x5FFF, placeholder);
        busA.RegisterBusRegion(0x80, 0xBF, 0x2000, 0x5FFF, placeholder);
    }
}