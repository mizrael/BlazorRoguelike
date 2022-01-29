using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace BlazorRoguelike.Web.Game.DungeonGenerator
{
    public class Dungeon : Map
    {
        #region Fields

        private readonly List<Point> visitedCells = new List<Point>();
        private readonly List<Room> rooms = new List<Room>();

        #endregion

        #region Constructors

        public Dungeon(int width, int height) : base(width, height)
        {

        }

        #endregion

        #region Methods

        internal void AddRoom(Room room)
        {
            rooms.Add(room);
        }

        public void FlagAllCellsAsUnvisited()
        {
            foreach(Point location in CellLocations)
                this[location].Visited = false;
        }

        public Point PickRandomCellAndFlagItAsVisited()
        {
            Point randomLocation = new Point(Random.Instance.Next(Width - 1), Random.Instance.Next(Height - 1));
            FlagCellAsVisited(randomLocation);
            return randomLocation;
        }

        public bool AdjacentCellInDirectionIsVisited(Point location, DirectionType direction)
        {
            Point? target = GetTargetLocation(location, direction);
            
            if (target == null)
                return false;

            switch (direction)
            {
                case DirectionType.North:
                    return this[target.Value].Visited;
                case DirectionType.West:
                    return this[target.Value].Visited;
                case DirectionType.South:
                    return this[target.Value].Visited;
                case DirectionType.East:
                    return this[target.Value].Visited;
                default:
                    throw new InvalidOperationException();
            }
        }

        public bool AdjacentCellInDirectionIsCorridor(Point location, DirectionType direction)
        {
            Point? target = GetTargetLocation(location, direction);

            if (target == null)
                return false;

            switch (direction)
            {
                case DirectionType.North:
                    return this[target.Value].IsCorridor;
                case DirectionType.West:
                    return this[target.Value].IsCorridor;
                case DirectionType.South:
                    return this[target.Value].IsCorridor;
                case DirectionType.East:
                    return this[target.Value].IsCorridor;
                default:
                    return false;
            }
        }

        public void FlagCellAsVisited(Point location)
        {
            if (!Bounds.Contains(location)) throw new ArgumentException("Location is outside of Dungeon bounds", "location");
            if (this[location].Visited) throw new ArgumentException("Location is already visited", "location");

            this[location].Visited = true;
            visitedCells.Add(location);
        }

        public Point GetRandomVisitedCell(Point location)
        {
            if (visitedCells.Count == 0) throw new InvalidOperationException("There are no visited cells to return.");

            int index = Random.Instance.Next(visitedCells.Count - 1);

            // Loop while the current cell is the visited cell
            while (visitedCells[index] == location)
                index = Random.Instance.Next(visitedCells.Count - 1);

            return visitedCells[index];
        }

        public Point CreateCorridor(Point location, DirectionType direction)
        {
            Point targetLocation = CreateSide(location, direction, SideType.Empty);
            this[location].IsCorridor = true; // Set current location to corridor
            this[targetLocation].IsCorridor = true; // Set target location to corridor
            return targetLocation;
        }

        public Point CreateWall(Point location, DirectionType direction)
        {
            return CreateSide(location, direction, SideType.Wall);
        }

        public Point CreateDoor(Point location, DirectionType direction)
        {
            return CreateSide(location, direction, SideType.Door);
        }

        private Point CreateSide(Point location, DirectionType direction, SideType sideType)
        {
            Point? target = GetTargetLocation(location, direction);
            if (target == null) throw new ArgumentException("There is no adjacent cell in the given direction", "location");

            switch (direction)
            {
                case DirectionType.North:
                    this[location].NorthSide = sideType;
                    this[target.Value].SouthSide = sideType;
                    break;
                case DirectionType.South:
                    this[location].SouthSide = sideType;
                    this[target.Value].NorthSide = sideType;
                    break;
                case DirectionType.West:
                    this[location].WestSide = sideType;
                    this[target.Value].EastSide = sideType;
                    break;
                case DirectionType.East:
                    this[location].EastSide = sideType;
                    this[target.Value].WestSide = sideType;
                    break;
            }

            return target.Value;
        }

        public TileType[,] ExpandToTiles(int tileStep)
        {
            int w = tileStep * (this.Width * 2 + 1);
            int h = tileStep * (this.Height * 2 + 1);

			// Instantiate our tile array
			TileType[,] tiles = new TileType[w, h];

            // Initialize the tile array to rock
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    tiles[x, y] = TileType.Void;

            // Fill tiles with corridor values for each room in dungeon
            foreach (Room room in this.Rooms)
            {
                var (minPoint, maxPoint) = room.GetBounds(tileStep);

                // Fill the room in tile space with an empty value
                for (int i = minPoint.X; i < maxPoint.X; i++)
                    for (int j = minPoint.Y; j < maxPoint.Y; j++)
                        tiles[i, j] = TileType.Empty;

                //tiles[minPoint.X - 1, minPoint.Y - 1] = TileType.WallAngleSE;
                //tiles[minPoint.X - 1, maxPoint.Y] = TileType.WallAngleNE;
                //tiles[maxPoint.X, minPoint.Y - 1] = TileType.WallAngleSO;
                //tiles[maxPoint.X, maxPoint.Y] = TileType.WallAngleNO;
            }

            int doorSize = 1;
            int startStep = (int)Math.Ceiling((decimal)doorSize / 2);

            // Loop for each corridor cell and expand it
            foreach (Point cellLocation in this.CorridorCellLocations)
            {
                Point tileLocation = new Point(tileStep * (cellLocation.X * 2 + 1), tileStep * (cellLocation.Y * 2 + 1));

                for (int x = tileLocation.X; x != tileLocation.X + tileStep; ++x)
                    for (int y = tileLocation.Y; y != tileLocation.Y + tileStep; ++y)
                        tiles[x, y] = TileType.Empty;

                if (this[cellLocation].SouthSide == SideType.Empty)
                {
                    for (int x = tileLocation.X; x != tileLocation.X + tileStep; ++x)
                        for (int y = tileLocation.Y + tileStep; y != tileLocation.Y + tileStep * 2 + 1; ++y)
                            tiles[x, y] = TileType.Empty;
                }
                if (this[cellLocation].EastSide == SideType.Empty)
                {
                    for (int x = tileLocation.X + tileStep; x != tileLocation.X + 1 + 2 * tileStep; ++x)
                        for (int y = tileLocation.Y; y != tileLocation.Y + tileStep; ++y)
                            tiles[x, y] = TileType.Empty;
                }

                if (this[cellLocation].NorthSide == SideType.Door)
                {
                    for (int x = tileLocation.X; x != tileLocation.X + tileStep; ++x)
                        for (int y = tileLocation.Y - tileStep - 1; y != tileLocation.Y; ++y)
                            tiles[x, y] = TileType.Empty;

                    for (int x = tileLocation.X; x != tileLocation.X + tileStep; ++x)
                        tiles[x, tileLocation.Y - 1] = TileType.Wall;

                    for (int x = tileLocation.X + startStep; x != tileLocation.X + startStep + doorSize; ++x)
                        tiles[x, tileLocation.Y - 1] = TileType.Door;
                }
                if (this[cellLocation].WestSide == SideType.Door)
                {
                    for (int x = tileLocation.X - tileStep - 1; x != tileLocation.X; ++x)
                        for (int y = tileLocation.Y; y != tileLocation.Y + tileStep; ++y)
                            tiles[x - 1, y] = TileType.Empty;

                    for (int y = tileLocation.Y; y != tileLocation.Y + tileStep; ++y)
                        tiles[tileLocation.X - 1, y] = TileType.Wall;

                    for (int y = tileLocation.Y + startStep; y != tileLocation.Y + startStep + doorSize; ++y)
                        tiles[tileLocation.X - 1, y] = TileType.Door;
                }
                // ******************************* //
                // original code 
                //if (dungeon[cellLocation].NorthSide == SideType.Empty) tiles[tileLocation.X, tileLocation.Y - 1] = TileType.Empty;
                //if (dungeon[cellLocation].NorthSide == SideType.Door) tiles[tileLocation.X, tileLocation.Y - 1] = TileType.Door;

                //if (dungeon[cellLocation].SouthSide == SideType.Empty) tiles[tileLocation.X, tileLocation.Y + 1] = TileType.Empty;
                //if (dungeon[cellLocation].SouthSide == SideType.Door) tiles[tileLocation.X, tileLocation.Y + 1] = TileType.Door;

                //if (dungeon[cellLocation].WestSide == SideType.Empty) tiles[tileLocation.X - 1, tileLocation.Y] = TileType.Empty;
                //if (dungeon[cellLocation].WestSide == SideType.Door) tiles[tileLocation.X - 1, tileLocation.Y] = TileType.Door;

                //if (dungeon[cellLocation].EastSide == SideType.Empty) tiles[tileLocation.X + 1, tileLocation.Y] = TileType.Empty;
                //if (dungeon[cellLocation].EastSide == SideType.Door) tiles[tileLocation.X + 1, tileLocation.Y] = TileType.Door; 
                // ******************************* //
            }

            // mark corners
            for (int x = 1; x < w - 1; x++)
            {
                for (int y = 1; y < h - 1; y++)
                {
                    if (TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x, y - 1]) &&
                         (tiles[x + 1, y] == TileType.Empty) && (tiles[x, y + 1] == TileType.Empty))
                    {
                        if (TileIsWall(tiles[x, y]))
                            tiles[x, y] = TileType.WallNO;
                        else if (tiles[x, y] == TileType.Empty)
                        {
                            tiles[x - 1, y - 1] = TileType.WallSE;
                            tiles[x - 1, y] = TileType.WallNS;
                            tiles[x, y - 1] = TileType.WallEO;
                        }
                    }

                    if (TileIsWall(tiles[x + 1, y]) && TileIsWall(tiles[x, y - 1]) &&
                       (tiles[x - 1, y] == TileType.Empty) && (tiles[x, y + 1] == TileType.Empty))
                    {
                        if (TileIsWall(tiles[x, y]))
                            tiles[x, y] = TileType.WallNE;
                        else if (tiles[x, y] == TileType.Empty)
                        {
                            tiles[x + 1, y - 1] = TileType.WallSO;
                            tiles[x + 1, y] = TileType.WallNS;
                            tiles[x, y - 1] = TileType.WallEO;
                        }
                    }

                    if (TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x, y + 1]) &&
                        (tiles[x + 1, y] == TileType.Empty) && (tiles[x, y - 1] == TileType.Empty))
                    {
                        if (TileIsWall(tiles[x, y]))
                            tiles[x, y] = TileType.WallSO;
                        else if (tiles[x, y] == TileType.Empty)
                        {
                            tiles[x - 1, y + 1] = TileType.WallNE;
                            tiles[x - 1, y] = TileType.WallNS;
                            tiles[x, y + 1] = TileType.WallEO;
                        }
                    }

                    if (TileIsWall(tiles[x + 1, y]) && TileIsWall(tiles[x, y + 1]) &&
                       (tiles[x - 1, y] == TileType.Empty) && (tiles[x, y - 1] == TileType.Empty))
                    {
                        if (TileIsWall(tiles[x, y]))
                            tiles[x, y] = TileType.WallSE;
                        else if (tiles[x, y] == TileType.Empty)
                        {
                            tiles[x + 1, y + 1] = TileType.WallNO;
                            tiles[x + 1, y] = TileType.WallNS;
                            tiles[x, y + 1] = TileType.WallEO;
                        }
                    }

                    if ((TileIsWall(tiles[x - 1, y - 1]) && TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x - 1, y + 1]) &&
                         TileIsWall(tiles[x, y - 1]) && TileIsWall(tiles[x, y + 1]) &&
                         (tiles[x + 1, y - 1] == TileType.Empty) && (tiles[x + 1, y] == TileType.Empty) && (tiles[x + 1, y + 1] == TileType.Empty)) ||
                        (TileIsWall(tiles[x + 1, y - 1]) && TileIsWall(tiles[x + 1, y]) && TileIsWall(tiles[x + 1, y + 1]) &&
                         TileIsWall(tiles[x, y - 1]) && TileIsWall(tiles[x, y + 1]) &&
                         (tiles[x - 1, y - 1] == TileType.Empty) && (tiles[x - 1, y] == TileType.Empty) && (tiles[x - 1, y + 1] == TileType.Empty)))
                    {
                        tiles[x, y] = TileType.WallNS;
                    }
                    else if ((TileIsWall(tiles[x - 1, y - 1]) && TileIsWall(tiles[x, y - 1]) && TileIsWall(tiles[x + 1, y - 1]) &&
                         TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x + 1, y]) &&
                         (tiles[x - 1, y + 1] == TileType.Empty) && (tiles[x, y + 1] == TileType.Empty) && (tiles[x + 1, y + 1] == TileType.Empty)) ||
                        (TileIsWall(tiles[x - 1, y + 1]) && TileIsWall(tiles[x, y + 1]) && TileIsWall(tiles[x + 1, y + 1]) &&
                         TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x + 1, y]) &&
                         (tiles[x - 1, y - 1] == TileType.Empty) && (tiles[x, y - 1] == TileType.Empty) && (tiles[x + 1, y - 1] == TileType.Empty)))
                    {
                        tiles[x, y] = TileType.WallEO;
                    }
                }
            }

            // mark three-way walls
            for (int x = 1; x < w - 1; x++)
            {
                for (int y = 1; y < h - 1; y++)
                {
                    if (TileIsWall(tiles[x - 1, y - 1]) && TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x - 1, y + 1]) &&
                         TileIsWall(tiles[x, y - 1]) && TileIsWall(tiles[x, y]) && TileIsWall(tiles[x, y + 1]) &&
                         (tiles[x + 1, y - 1] == TileType.Empty) && TileIsWall(tiles[x + 1, y]) && (tiles[x + 1, y + 1] == TileType.Empty))
                    {
                        tiles[x, y] = TileType.WallNES;
                    }
                    else if (TileIsWall(tiles[x + 1, y - 1]) && TileIsWall(tiles[x + 1, y]) && TileIsWall(tiles[x + 1, y + 1]) &&
                         TileIsWall(tiles[x, y - 1]) && TileIsWall(tiles[x, y]) && TileIsWall(tiles[x, y + 1]) &&
                         (tiles[x - 1, y - 1] == TileType.Empty) && TileIsWall(tiles[x - 1, y]) && (tiles[x - 1, y + 1] == TileType.Empty))
                    {
                        tiles[x, y] = TileType.WallNSO;
                    }
                    else if (TileIsWall(tiles[x - 1, y - 1]) && TileIsWall(tiles[x, y - 1]) && TileIsWall(tiles[x + 1, y - 1]) &&
                        TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x, y]) && TileIsWall(tiles[x + 1, y]) &&
                         (tiles[x - 1, y + 1] == TileType.Empty) && TileIsWall(tiles[x, y + 1]) && (tiles[x + 1, y + 1] == TileType.Empty))
                    {
                        tiles[x, y] = TileType.WallESO;
                    }
                    else if (TileIsWall(tiles[x - 1, y + 1]) && TileIsWall(tiles[x, y + 1]) && TileIsWall(tiles[x + 1, y + 1]) &&
                        TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x, y]) && TileIsWall(tiles[x + 1, y]) &&
                         (tiles[x - 1, y - 1] == TileType.Empty) && TileIsWall(tiles[x, y - 1]) && (tiles[x + 1, y - 1] == TileType.Empty))
                    {
                        tiles[x, y] = TileType.WallNEO;
                    }

                    else if (TileIsWall(tiles[x, y - 1]) &&
                            ((tiles[x - 1, y - 1] == TileType.Empty) && TileIsWall(tiles[x + 1, y - 1]) || (tiles[x + 1, y - 1] == TileType.Empty) && TileIsWall(tiles[x - 1, y - 1])) &&
                             TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x, y]) && TileIsWall(tiles[x + 1, y]) &&
                             (tiles[x - 1, y + 1] == TileType.Empty) && (tiles[x, y + 1] == TileType.Empty) && (tiles[x + 1, y + 1] == TileType.Empty))
                    {
                        tiles[x, y] = TileType.WallNEO;
                    }

                    else if ((tiles[x - 1, y - 1] == TileType.Empty) && TileIsWall(tiles[x, y - 1]) && (tiles[x + 1, y - 1] == TileType.Empty) &&
                             TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x, y]) && (tiles[x + 1, y] == TileType.Empty) &&
                             TileIsWall(tiles[x - 1, y + 1]) && TileIsWall(tiles[x, y + 1]) && (tiles[x + 1, y + 1] == TileType.Empty))
                    {
                        tiles[x, y] = TileType.WallNSO;
                    }
                    else if (TileIsWall(tiles[x - 1, y - 1]) && TileIsWall(tiles[x, y - 1]) && (tiles[x + 1, y - 1] == TileType.Empty) &&
                         TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x, y]) && (tiles[x + 1, y] == TileType.Empty) &&
                         (tiles[x - 1, y + 1] == TileType.Empty) && TileIsWall(tiles[x, y + 1]) && (tiles[x + 1, y + 1] == TileType.Empty))
                    {
                        tiles[x, y] = TileType.WallNSO;
                    }

                    else if ((TileIsWall(tiles[x - 1, y - 1]) || (tiles[x - 1, y - 1] == TileType.Empty)) &&
                            TileIsWall(tiles[x, y - 1]) && (tiles[x + 1, y - 1] == TileType.Empty) &&
                            TileIsWall(tiles[x - 1, y]) && TileIsWall(tiles[x, y]) && TileIsWall(tiles[x + 1, y]) &&
                            (tiles[x - 1, y + 1] == TileType.Empty) && TileIsWall(tiles[x, y + 1]) && (tiles[x + 1, y + 1] == TileType.Empty))
                    {
                        tiles[x, y] = TileType.WallNESO;
                    }
                }
            }

            // remove the doors in the middle of a room
            /*  for (int x = 0; x < w; x++)
              {
                  for (int y = 0; y < h; y++)
                  {
                      if (tiles[x, y] != TileType.Door) continue;

                      if (tiles[x - 1, y] == TileType.Wall) continue;
                      if (tiles[x, y - 1] == TileType.Wall) continue;
                      if (tiles[x - 1, y - 1] == TileType.Wall) continue;

                      if (tiles[x + 1, y] == TileType.Wall) continue;
                      if (tiles[x, y + 1] == TileType.Wall) continue;
                      if (tiles[x + 1, y + 1] == TileType.Wall) continue;

                      if (tiles[x - 1, y + 1] == TileType.Wall) continue;
                      if (tiles[x + 1, y - 1] == TileType.Wall) continue;

                      tiles[x, y] = TileType.Empty;
                  }
              }
              // clean up doors glitches
              for (int x = 0; x < w; x++)
              {
                  for (int y = 0; y < h; y++)
                  {
                      if (tiles[x, y] != TileType.Door) continue;

                      if (tiles[x - 1, y] == TileType.Wall && tiles[x + 1, y] == TileType.Empty)
                      {
                          tiles[x, y] = TileType.Empty;
                          continue;
                      }
                      if (tiles[x + 1, y] == TileType.Wall && tiles[x - 1, y] == TileType.Empty)
                      {
                          tiles[x, y] = TileType.Empty;
                          continue;
                      }
                      if (tiles[x, y - 1] == TileType.Wall && tiles[x, y + 1] == TileType.Empty)
                      {
                          tiles[x, y] = TileType.Empty;
                          continue;
                      }
                      if (tiles[x, y + 1] == TileType.Wall && tiles[x, y - 1] == TileType.Empty)
                      {
                          tiles[x, y] = TileType.Empty;
                          continue;
                      }
                  }
              }*/

            return tiles;
        }

        private static bool TileIsWall(TileType tile)
        {
            return (tile >= TileType.Wall && tile <= TileType.WallNESO) || tile == TileType.Void;
        }

        #endregion

        #region Properties

        public ReadOnlyCollection<Room> Rooms
        {
            get { return rooms.AsReadOnly(); }
        }

        public IEnumerable<Point> DeadEndCellLocations
        {
            get
            {
                foreach (Point point in CellLocations)
                    if (this[point].IsDeadEnd) yield return point;
            }
        }

        public IEnumerable<Point> CorridorCellLocations
        {
            get
            {
                foreach (Point point in CellLocations)
                    if (this[point].IsCorridor) yield return point;

            }
        }

        public bool AllCellsAreVisited
        {
            get { return visitedCells.Count == (Width*Height); }
        }

        #endregion
    }
}