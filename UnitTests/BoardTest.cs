using Battleship.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Battleship
{

    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void AttackComputer_Hit_Works()
        {
            GameController ctrl = SetupBoards();
            var result = ctrl.AttackComputer(new Location(0, 0));
            Assert.IsTrue(result == AttackResult.Hit);
        }

        [TestMethod]
        public void AttackComputer_Hit_Recorded()
        {
            GameController ctrl = SetupBoards();
            var result = ctrl.AttackComputer(new Location(0, 0));
            Assert.IsTrue(ctrl.ComputerBoard.Cells[0, 0] == CellType.Hit);
        }

        [TestMethod]
        public void AttackComputer_Miss_Works()
        {
            GameController ctrl = SetupBoards();
            var result = ctrl.AttackComputer(new Location(1, 0));
            Assert.IsTrue(result == AttackResult.Miss);
        }

        [TestMethod]
        public void AttackComputer_Miss_Recorded()
        {
            GameController ctrl = SetupBoards();
            var result = ctrl.AttackComputer(new Location(1, 0));
            Assert.IsTrue(ctrl.ComputerBoard.Cells[1, 0] == CellType.Miss);
        }

        [TestMethod]
        public void AttackComputer_Sink_Works()
        {
            GameController ctrl = SetupBoards();
            ctrl.AttackComputer(new Location(0, 0));
            ctrl.AttackComputer(new Location(0, 1));
            var result = ctrl.AttackComputer(new Location(0, 2));
            Assert.IsTrue(result == AttackResult.Sink);
        }

        [TestMethod]
        public void AttackComputer_Repeat_Works()
        {
            GameController ctrl = SetupBoards();
            ctrl.AttackComputer(new Location(0, 0));
            var result = ctrl.AttackComputer(new Location(0, 0));
            Assert.IsTrue(result == AttackResult.repeat);
        }

        [TestMethod]
        public void AddShip_Horizontal_Success()
        {
            var ctrl = new GameController(5);
            ctrl.ComputerBoard.AddShip(new Ship() { Row = 0, Col = 0, Orientation = ShipDirection.Horizontal, Length = 4 });
            Assert.IsTrue(ctrl.ComputerBoard.Cells[0, 0] == CellType.Ship);
            Assert.IsTrue(ctrl.ComputerBoard.Cells[0, 1] == CellType.Ship);
            Assert.IsTrue(ctrl.ComputerBoard.Cells[0, 2] == CellType.Ship);
            Assert.IsTrue(ctrl.ComputerBoard.Cells[0, 3] == CellType.Ship);
            Assert.IsTrue(ctrl.ComputerBoard.Cells[0, 4] == CellType.Water);

        }

        [TestMethod]
        public void AddShip_Vertical_Success()
        {
            var ctrl = new GameController(5);
            ctrl.ComputerBoard.AddShip(new Ship() { Row = 0, Col = 0, Orientation = ShipDirection.Vertical, Length = 4 });
            Assert.IsTrue(ctrl.ComputerBoard.Cells[0, 0] == CellType.Ship);
            Assert.IsTrue(ctrl.ComputerBoard.Cells[1, 0] == CellType.Ship);
            Assert.IsTrue(ctrl.ComputerBoard.Cells[2, 0] == CellType.Ship);
            Assert.IsTrue(ctrl.ComputerBoard.Cells[3, 0] == CellType.Ship);
            Assert.IsTrue(ctrl.ComputerBoard.Cells[4, 0] == CellType.Water);

        }

        [TestMethod]
        public void AddShip_NoShip_Exception()
        {
            var ctrl = new GameController(5);
            try
            {
                ctrl.ComputerBoard.AddShip(null);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message == "Can't place ship.");
            }

        }

        [TestMethod]
        public void AddShip_TooBig_Exception()
        {
            var ctrl = new GameController(5);
            try
            {
                ctrl.ComputerBoard.AddShip(new Ship() { Row = 0, Col = 0, Orientation = ShipDirection.Horizontal, Length = 1000 });
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message == "Can't place ship.");
            }
        }

        [TestMethod]
        public void AddShip_TooSmall_Exception()
        {
            var ctrl = new GameController(5);
            try
            {
                ctrl.ComputerBoard.AddShip(new Ship() { Row = 0, Col = 0, Orientation = ShipDirection.Horizontal, Length = 0 });
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message == "Can't place ship.");
            }
        }

        [TestMethod]
        public void IsGameOver_True()
        {
            var ctrl = SetupBoards();
            ctrl.AttackComputer(new Location(0, 0));
            ctrl.AttackComputer(new Location(0, 1));
            ctrl.AttackComputer(new Location(0, 2));

            Assert.IsTrue(ctrl.ComputerBoard.AllShipsSunk());
            Assert.IsTrue(ctrl.IsGameOver());
        }

        [TestMethod]
        public void IsGameOver_False()
        {
            var ctrl = SetupBoards();
            ctrl.AttackComputer(new Location(0, 0));

            Assert.IsTrue(!ctrl.ComputerBoard.AllShipsSunk());
            Assert.IsTrue(!ctrl.IsGameOver());
        }
        
        [TestMethod]
        public void FindShip_Horizontal_Success()
        {
            var ctrl = SetupBoards();
            int res1 = ctrl.ComputerBoard.FindShip(new Location(0, 1));
            int res2 = ctrl.ComputerBoard.FindShip(new Location(0, 2));

            Assert.IsTrue(res1 == 0);
            Assert.IsTrue(res2 == 0);
        }

        [TestMethod]
        public void FindShip_Vertical_Success()
        {
            GameController ctrl = new GameController(5);
            ctrl.ComputerBoard.AddShip(new Ship() { Row = 0, Col = 0, Orientation = ShipDirection.Vertical, Length = 3 });
            int res1 = ctrl.ComputerBoard.FindShip(new Location(0, 0));
            int res2 = ctrl.ComputerBoard.FindShip(new Location(2, 0));

            Assert.IsTrue(res1 == 0);
            Assert.IsTrue(res2 == 0);
        }

        [TestMethod]
        public void FindShip_NotFound_Exception()
        {
            var ctrl = SetupBoards();
            try
            {
                ctrl.ComputerBoard.FindShip(new Location(6, 4));
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message == "It's not supposed to get here.");
            }
        }

        GameController SetupBoards()
        {
            var ctrl = new GameController(10);
            ctrl.ComputerBoard.AddShip(new Ship() { Row = 0, Col = 0, Orientation = ShipDirection.Horizontal, Length = 3 });
            ctrl.PlayerBoard.AddShip(new Ship() { Row = 0, Col = 0, Orientation = ShipDirection.Horizontal, Length = 3 });
            return ctrl;
        }
    }
}
