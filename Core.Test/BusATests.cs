using Shouldly;
using Xunit;
using NSubstitute;

namespace Core.Test;

public class BusATests
{
    [Fact]
    public void ReadUnassignedRegionThrows()
    {
        //Arrange
        BusA busA = new BusA();
        //Act
        Exception? exception = Record.Exception(() => busA.Read(0x0000));
        //Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<NullReferenceException>();
    }
    
    [Fact]
    public void AssignRegionAllowsReadingWholeRegion()
    {
        //Arrange
        BusA busA = new BusA();
        IBusADevice sub = Substitute.For<IBusADevice>();
        sub.Read(Arg.Any<uint>()).Returns((byte)0xFF);
        busA.RegisterMemoryRegion(0x00, 0x00, 0x0000, 0x2000, sub);
        
        //Act & Assert
        for (uint i = 0; i < 0x2000; i++)
        {
            busA.Read(i).ShouldBe((byte)0xFF);
        }
    }
    
    [Fact]
    public void AssignSpecificRegionAllowsReadingInOnlyThatRegion()
    {
        //Arrange
        BusA busA = new BusA();
        IBusADevice sub = Substitute.For<IBusADevice>();
        sub.Read(Arg.Any<uint>()).Returns((byte)0xFF);
        
        byte bankFrom = 0x10;
        byte bankTo = 0x12;
        ushort addressFrom = 0x4000;
        ushort addressTo = 0x8000;
        busA.RegisterMemoryRegion(bankFrom, bankTo, addressFrom, addressTo, sub);
        
        for (uint i = 0; i < 0x1000000; i++)
        {
            uint lowerBytes = i & 0xFFF;
            if (lowerBytes is not 0x000 and not 0xFFF)
                continue; //to speed up the test
            uint bank = i >> 16;
            uint address = i & 0xFFFF;
            //Act
            Exception? exception = Record.Exception(() => busA.Read(i));
            //Assert
            if (bank >= bankFrom && bank <= bankTo && address >= addressFrom && address < addressTo)
            {
                //valid read area
                exception.ShouldBeNull($"Should be able to read at 0x{i.ToString("X6")} but could not.");
            }
            else
            {
                //invalid read area
                exception.ShouldNotBeNull($"Should not be able to read at 0x{i.ToString("X6")} but could.");
            }
        }
    }
}