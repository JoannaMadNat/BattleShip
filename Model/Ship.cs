using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{

    public enum ShipDirection
    {
        Horizontal, Vertical
    }

    public class Ship
    {
        public int Row { get; set; } // upper-left

        public int Col { get; set; } // upper-left

        public int Length { get; set; }
        
        public ShipDirection Orientation { get; set; }
         
        public int Damage { get; set; }

        public Ship()
        {
            Damage = 0;
            Orientation = ShipDirection.Horizontal;
        }
    }
}
