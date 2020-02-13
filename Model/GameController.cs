using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Battleship.Model
{
    public enum AttackResult
    {
        Hit, Miss, Sink, repeat
    }

    public class GameController
    {
        public Board PlayerBoard { get; set; }
        public Board ComputerBoard { get; set; }

        public GameController(int size)
        {
            PlayerBoard = new Board(size);
            ComputerBoard = new Board(size);
        }

        public void Setup()
        {
            PlayerBoard.PlaceShips();
            ComputerBoard.PlaceShips();            
        }

        public AttackResult AttackComputer(Location loc)
        {
            return ComputerBoard.Attack(loc);
        } 

        public bool IsGameOver()
        {
            return ComputerBoard.AllShipsSunk() || PlayerBoard.AllShipsSunk();
        }

        public ComputerAttackResult AttackPlayer()
        {
            ComputerAttackResult compu = new ComputerAttackResult();
            compu.Row = RandomManager.GetNext(0, PlayerBoard.Size);
            compu.Col = RandomManager.GetNext(0, PlayerBoard.Size);
            compu.Result = PlayerBoard.Attack(new Location( compu.Row, compu.Col));
            while (compu.Result == AttackResult.repeat)
            {
                compu.Row = RandomManager.GetNext(0, PlayerBoard.Size);
                compu.Col = RandomManager.GetNext(0, PlayerBoard.Size);
                compu.Result = PlayerBoard.Attack(new Location(compu.Row, compu.Col));
            }
                return compu;
        }
    }

    public class ComputerAttackResult
    {
        public int Row { get; set; }

        public int Col { get; set; }

        public AttackResult Result { get; set; }
    }
}
