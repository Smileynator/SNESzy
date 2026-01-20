using Shouldly;
using Xunit;

namespace Core.Test;

public class WorkRamTest
{
    [Fact]
    public void FullRamRangeAccess()
    {
        //Arrange
        WorkRam ram = new WorkRam();
        Random r = new Random(1337);
        //Act
        for (uint i = 0x7E0000; i < 0x800000; i++)
        {
            ram.BusAWrite(i, (byte)r.Next());
        }
        //Assert
        r = new Random(1337);
        for (uint i = 0x7E0000; i < 0x800000; i++)
        {
            byte read = ram.BusARead(i);
            read.ShouldBe((byte)r.Next());
        }
    }
    
    [Fact]
    public void LowRamAccessReadEverywhere()
    {
        //Arrange
        WorkRam ram = new WorkRam();
        //Act
        ram.BusAWrite(0x7E0000, 0xDE);
        ram.BusAWrite(0x7E1FFF, 0xAD);
        ram.BusAWrite(0x7E1000, 0xBE);
        ram.BusAWrite(0x7E1111, 0xEF);
        //Assert
        for (byte bank = 0x00; bank < 0x40; bank++)
        {
            var b = bank << 16;
            ram.BusARead((uint) b | 0x0000).ShouldBe( (byte) 0xDE);
            ram.BusARead((uint) b | 0x1FFF).ShouldBe( (byte) 0xAD);
            ram.BusARead((uint) b | 0x1000).ShouldBe( (byte) 0xBE);
            ram.BusARead((uint) b | 0x1111).ShouldBe( (byte) 0xEF);
        }
        for (byte bank = 0x80; bank < 0xC0; bank++)
        {
            var b = bank << 16;
            ram.BusARead((uint) b | 0x0000).ShouldBe( (byte) 0xDE);
            ram.BusARead((uint) b | 0x1FFF).ShouldBe( (byte) 0xAD);
            ram.BusARead((uint) b | 0x1000).ShouldBe( (byte) 0xBE);
            ram.BusARead((uint) b | 0x1111).ShouldBe( (byte) 0xEF);
        }
    }
    
    [Fact]
    public void LowRamAccessWriteEverywhere()
    {
        //Arrange
        WorkRam ram = new WorkRam();
        for (byte bank = 0x00; bank < 0x40; bank++)
        {
            var b = bank << 16;
            //Act
            ram.BusAWrite((uint) b | 0x0000, bank);
            ram.BusAWrite((uint) b | 0x1FFF, (byte) (bank+1));
            ram.BusAWrite((uint) b | 0x1000, (byte) (bank+2));
            ram.BusAWrite((uint) b | 0x1111, (byte) (bank+3));
            //Assert
            ram.BusARead(0x7E0000).ShouldBe( bank);
            ram.BusARead(0x7E1FFF).ShouldBe( (byte) (bank+1));
            ram.BusARead(0x7E1000).ShouldBe( (byte) (bank+2));
            ram.BusARead(0x7E1111).ShouldBe( (byte) (bank+3));
        }
        for (byte bank = 0x80; bank < 0xC0; bank++)
        {
            var b = bank << 16;
            //Act
            ram.BusAWrite((uint) b | 0x0000, bank);
            ram.BusAWrite((uint) b | 0x1FFF, (byte) (bank+1));
            ram.BusAWrite((uint) b | 0x1000, (byte) (bank+2));
            ram.BusAWrite((uint) b | 0x1111, (byte) (bank+3));
            //Assert
            ram.BusARead(0x7E0000).ShouldBe( bank);
            ram.BusARead(0x7E1FFF).ShouldBe( (byte) (bank+1));
            ram.BusARead(0x7E1000).ShouldBe( (byte) (bank+2));
            ram.BusARead(0x7E1111).ShouldBe( (byte) (bank+3));
        }
    }
    
    [Fact]
    public void InvalidAreaShouldThrow()
    {
        //Arrange
        WorkRam ram = new WorkRam();
        //Act
        Exception? exception = Record.Exception(() => ram.BusARead(0x400000));
        //Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeAssignableTo<ArgumentOutOfRangeException>();
        //Act
        exception = Record.Exception(() => ram.BusARead(0x002000));
        //Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeAssignableTo<ArgumentOutOfRangeException>();
    }
}