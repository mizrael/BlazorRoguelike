using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BlazorRoguelike.Web.Game.DungeonGenerator
{
    public class DungeonGenerator
    {
        #region Fields

        private int width = 25;
        private int height = 25;
        private int changeDirectionModifier = 30;
        private int sparsenessModifier = 70;
        private int deadEndRemovalModifier = 50;
        private readonly RoomGenerator roomGenerator = new RoomGenerator(10, 1, 5, 1, 5);

        #endregion

        #region Constructors

        public DungeonGenerator(int width, int height, int changeDirectionModifier, int sparsenessModifier, int deadEndRemovalModifier, RoomGenerator roomGenerator)
        {
            this.width = width;
            this.height = height;
            this.changeDirectionModifier = changeDirectionModifier;
            this.sparsenessModifier = sparsenessModifier;
            this.deadEndRemovalModifier = deadEndRemovalModifier;
            this.roomGenerator = roomGenerator;
        }

        #endregion

        #region Methods

        public Dungeon Generate()
        {
            Dungeon dungeon = new Dungeon(width, height);
            dungeon.FlagAllCellsAsUnvisited();

            CreateDenseMaze(dungeon);
            SparsifyMaze(dungeon);
            RemoveDeadEnds(dungeon);
            roomGenerator.PlaceRooms(dungeon);
            roomGenerator.PlaceDoors(dungeon);

            return dungeon;
        }

        public void CreateDenseMaze(Dungeon dungeon)
        {
            Point currentLocation = dungeon.PickRandomCellAndFlagItAsVisited();
            DirectionType previousDirection = DirectionType.North;

            while (!dungeon.AllCellsAreVisited)
            {
                DirectionPicker directionPicker = new DirectionPicker(previousDirection, changeDirectionModifier);
                DirectionType direction = directionPicker.GetNextDirection();

                while (!dungeon.HasAdjacentCellInDirection(currentLocation, direction) || dungeon.AdjacentCellInDirectionIsVisited(currentLocation, direction))
                {
                    if (directionPicker.HasNextDirection)
                        direction = directionPicker.GetNextDirection();
                    else
                    {
                        currentLocation = dungeon.GetRandomVisitedCell(currentLocation); // Get a new previously visited location
                        directionPicker = new DirectionPicker(previousDirection, changeDirectionModifier); // Reset the direction picker
                        direction = directionPicker.GetNextDirection(); // Get a new direction
                    }
                }

                currentLocation = dungeon.CreateCorridor(currentLocation, direction);
                dungeon.FlagCellAsVisited(currentLocation);
                previousDirection = direction;
            }
        }

        public void SparsifyMaze(Dungeon dungeon)
        {
            // Calculate the number of cells to remove as a percentage of the total number of cells in the dungeon
            int noOfDeadEndCellsToRemove = (int)Math.Ceiling(((decimal)sparsenessModifier / 100) * (dungeon.Width * dungeon.Height));

            IEnumerator<Point> enumerator = dungeon.DeadEndCellLocations.GetEnumerator();

            for (int i = 0; i < noOfDeadEndCellsToRemove; i++)
            {
                if (!enumerator.MoveNext()) // Check if there is another item in our enumerator
                {
                    enumerator = dungeon.DeadEndCellLocations.GetEnumerator(); // Get a new enumerator
                    if (!enumerator.MoveNext()) break; // No new items exist so break out of loop
                }

                Point point = enumerator.Current;
                dungeon.CreateWall(point, dungeon[point].CalculateDeadEndCorridorDirection());
                dungeon[point].IsCorridor = false;
            }
        }

        public void RemoveDeadEnds(Dungeon dungeon)
        {
            foreach (Point deadEndLocation in dungeon.DeadEndCellLocations)
            {
                if (ShouldRemoveDeadend())
                {
                    Point currentLocation = deadEndLocation;

                    do
                    {
                        // Initialize the direction picker not to select the dead-end corridor direction
                        DirectionPicker directionPicker = new DirectionPicker(dungeon[currentLocation].CalculateDeadEndCorridorDirection(), 100);
                        DirectionType direction = directionPicker.GetNextDirection();

                        while (!dungeon.HasAdjacentCellInDirection(currentLocation, direction))
                        {
                            if (directionPicker.HasNextDirection)
                                direction = directionPicker.GetNextDirection();
                            else
                                throw new InvalidOperationException("This should not happen");
                        }
                        // Create a corridor in the selected direction
                        currentLocation = dungeon.CreateCorridor(currentLocation, direction);

                    } while (dungeon[currentLocation].IsDeadEnd); // Stop when you intersect an existing corridor.
                }
            }
        }

        public bool ShouldRemoveDeadend()
        {
            return Random.Instance.Next(1, 99) < deadEndRemovalModifier;
        }

        #endregion

        #region Properties

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int ChangeDirectionModifier
        {
            get { return changeDirectionModifier; }
            set { changeDirectionModifier = value; }
        }

        public int SparsenessModifier
        {
            get { return sparsenessModifier; }
            set { sparsenessModifier = value; }
        }

        public int DeadEndRemovalModifier
        {
            get { return deadEndRemovalModifier; }
            set { deadEndRemovalModifier = value; }
        }

        #endregion

    }
}