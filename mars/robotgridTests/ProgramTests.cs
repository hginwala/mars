using Microsoft.VisualStudio.TestTools.UnitTesting;
using robotgrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robotgrid.Tests
{
    [TestClass()]
        
    public class ProgramTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            if (Program.AvailableCommands == null || !Program.AvailableCommands.Any())
            {
                Program.AvailableCommands = new List<Command>()
                {
                    new Command {Code = "F",Magnitude =1, Name ="Forward",Type = CommandType.Movement },
                    new Command {Code = "R",Magnitude =90, Name ="Right",Type = CommandType.Orientation },
                    new Command {Code = "L",Magnitude =-90, Name ="Left",Type = CommandType.Orientation },
                };
            }
        }

        [TestMethod()]
        public void ExecuteCommandTestPass()
        {
            Program.grid = new int[5, 3];
            Program.CurrentPosition = new Position
            {
                x = 1,
                y = 1,
                Direction = Directions.E
            };
            var testints = "FF".ToCharArray();
            Program.MoveRobot(testints);
            var finalPos = new Position
            {
                x = 3,
                y = 1,
                Direction = Directions.E
            };
            Assert.AreEqual(finalPos, Program.CurrentPosition);           
        }

        [TestMethod()]
        public void ExecuteCommandTestSenseEdge()
        {
            Program.grid = new int[6, 4];
            Program.grid[3, 3] = -1;
            Program.CurrentPosition = new Position
            {
                x = 0,
                y = 3,
                Direction = Directions.W
            };
            var testints = "LLFFFLFLFL".ToCharArray();
            Program.MoveRobot(testints);
            var finalPos = new Position
            {
                x = 2,
                y = 3,
                Direction = Directions.S
            };
            Assert.AreEqual(finalPos, Program.CurrentPosition);          
        }
    }
}