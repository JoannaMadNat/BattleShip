using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace Battleship.Model
{
    public enum CellType
    {
        Ship, Water, Hit, Miss
    }

    public class Location
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Location(int row, int col)
        {
            Row = row;
            Column = col;
        }
    }

    public class Board
    {
        public int Size { get; set; }

        public List<Ship> Ships { get; set; }

        public CellType[,] Cells { get; set; }

        public Board(int size)
        {
            Size = size;
            Cells = new CellType[size, size];
            for (int row = 0; row < size; ++row)
                for (int col = 0; col < size; ++col)
                    Cells[row, col] = CellType.Water;

            Ships = new List<Ship>();
        }


        public void PlaceShips()
        {
            Ship s1 = new Ship();
            s1.Length = 1;
            Ship s2 = new Ship();
            s2.Length = 2;
            Ship s3 = new Ship();
            s3.Length = 4;

            List<Ship> toPlace = new List<Ship>();
            toPlace.Add(s1);
            toPlace.Add(s2);
            toPlace.Add(s3);

            for (int i = toPlace.Count - 1; i >= 0; i--)
            {
                bool isSuccess = PlaceShip(toPlace[i]);

                if (!isSuccess) //restart everything if loop takes too long
                {
                    Ships.Clear();
                    for (int row = 0; row < Size; ++row)
                        for (int col = 0; col < Size; ++col)
                            Cells[row, col] = CellType.Water;
                    i = toPlace.Count;
                }

            }
        }

        bool PlaceShip(Ship s)
        {
            int loopCounter = 0;
            bool valid = false;

            if (RandomManager.GetNext(0, 2) == 1)    //horizontal
            {
                s.Orientation = ShipDirection.Horizontal;

                while (!valid)
                {
                    ++loopCounter;
                    if (loopCounter > 10)
                        return false;
                    s.Row = RandomManager.GetNext(0, Size);
                    s.Col = RandomManager.GetNext(0, Size - s.Length);
                    valid = true;

                    for (int i = 0; i < Ships.Count; ++i)
                    {
                        if (Ships[i].Orientation == ShipDirection.Horizontal)
                        {
                            if (Math.Abs(Ships[i].Row - s.Row) < 2 || ((Ships[i].Col <= s.Col + s.Length && Ships[i].Col >= s.Col)
                                || (Ships[i].Col + Ships[i].Length <= s.Col + s.Length && Ships[i].Col + Ships[i].Length >= s.Col)))
                                valid = false;
                        }
                        else if (Ships[i].Orientation == ShipDirection.Vertical)
                        {
                            if (s.Row <= Ships[i].Row + Ships[i].Length && s.Row >= Ships[i].Row && Ships[i].Col <= s.Col + s.Length && Ships[i].Col >= s.Col)
                                valid = false;
                        }
                    }
                }
                if (valid)
                {
                    AddShip(s);
                }
            }
            else    //vertical
            {
                s.Orientation = ShipDirection.Vertical;

                while (!valid)
                {
                    ++loopCounter;
                    if (loopCounter > 10)
                        return false;
                    s.Row = RandomManager.GetNext(0, Size - s.Length);
                    s.Col = RandomManager.GetNext(0, Size);

                    valid = true;
                    for (int i = 0; i < Ships.Count; ++i)
                    {
                        if (Ships[i].Orientation == ShipDirection.Horizontal)
                        {
                            if (s.Col <= Ships[i].Col + Ships[i].Length && s.Col >= Ships[i].Col && Ships[i].Row <= s.Row+s.Length && Ships[i].Row >= s.Row)
                                valid = false;
                        }
                        else if (Ships[i].Orientation == ShipDirection.Vertical)
                        {
                            if (Math.Abs(Ships[i].Col - s.Col) < 2 || ((Ships[i].Row <= s.Row + s.Length && Ships[i].Row >= s.Row)
                                || (Ships[i].Row + Ships[i].Length <= s.Row + s.Length && Ships[i].Row + Ships[i].Length >= s.Row)))
                                valid = false;
                        }
                    }
                }
                if (valid)
                {
                    AddShip(s);
                }
            }

            return true;
        }

        public void AddShip(Ship s)
        {
            if (s == null || s.Length<=0 || s.Length >Size)
                throw new ArgumentException("Can't place ship.");

            if (s.Orientation == ShipDirection.Vertical)
            {
                Ships.Add(s);
                for (int i = 0; i < s.Length; i++)
                    Cells[s.Row + i, s.Col] = CellType.Ship;
            }
            else
            {
                Ships.Add(s);
                for (int i = 0; i < s.Length; i++)
                    Cells[s.Row, s.Col + i] = CellType.Ship;
            }

        }
        public AttackResult Attack(Location loc)
        {
            Debug.Assert(0 <= loc.Row && loc.Row < Size);
            Debug.Assert(0 <= loc.Column && loc.Column < Size);

            if (Cells[loc.Row, loc.Column] == CellType.Ship)
            {
                Cells[loc.Row, loc.Column] = CellType.Hit;
                int i = FindShip(loc);
                return HitShip(i);
            }
            else if (Cells[loc.Row, loc.Column] == CellType.Water)
            {
                Cells[loc.Row, loc.Column] = CellType.Miss;
                return AttackResult.Miss;
            }
            return AttackResult.repeat;
            // throw new InvalidAttackException();
        }

        public int FindShip(Location loc)
        {

            for (int i = 0; i < Ships.Count; i++)
            {
                if (Ships[i] != null)
                    if (Ships[i].Orientation == ShipDirection.Horizontal)
                    {
                        if (loc.Row == Ships[i].Row && ((loc.Column <= Ships[i].Col + Ships[i].Length && loc.Column >= Ships[i].Col) || loc.Column == Ships[i].Col))
                        {
                            return i;
                        }
                    }
                    else if (Ships[i].Orientation == ShipDirection.Vertical)
                        if (loc.Column == Ships[i].Col && ((loc.Row <= Ships[i].Row + Ships[i].Length && loc.Row >= Ships[i].Row) || loc.Row == Ships[i].Row))
                        {
                            return i;
                        }
            }
            throw new Exception("It's not supposed to get here.");
        }

        AttackResult HitShip(int i)
        {
            Ships[i].Damage++;

            if (Ships[i].Damage >= Ships[i].Length)
            {
                Ships.RemoveAt(i);
                return AttackResult.Sink;
            }
            return AttackResult.Hit;
        }

        public bool AllShipsSunk()
        {
            if (Ships.Count == 0)
                return true;
            return false;
        }

    }
}