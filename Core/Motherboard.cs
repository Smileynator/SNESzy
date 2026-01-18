namespace Core;

public class Motherboard
{
    //PAL clock speed 21.28137MHz
    private Cartridge cartridge;
    private BusA busA;
    private CPU cpu;

    public Motherboard(byte[] romData)
    {
        cartridge = new Cartridge(romData);
        busA = new BusA();
        cpu = new CPU(busA, busA);

        for (int i = 0; i < 100; i++)
        {
            cpu.Tick();
        }
    }
}