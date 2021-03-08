using System;

namespace BlazorRoguelike.Web.Game.DungeonGenerator
{
    public class Cell
    {
        #region Fields

        private SideType eastSide = SideType.Wall;
        private SideType northSide = SideType.Wall;
        private SideType southSide = SideType.Wall;
        private bool visited;
        private SideType westSide = SideType.Wall;
        private bool isCorridor;

        #endregion

        #region Properties

        public bool Visited
        {
            get { return visited; }
            set { visited = value; }
        }

        public SideType NorthSide
        {
            get { return northSide; }
            set { northSide = value; }
        }

        public SideType SouthSide
        {
            get { return southSide; }
            set { southSide = value; }
        }

        public SideType EastSide
        {
            get { return eastSide; }
            set { eastSide = value; }
        }

        public SideType WestSide
        {
            get { return westSide; }
            set { westSide = value; }
        }

        public bool IsDeadEnd
        {
            get { return WallCount == 3; }
        }

        public bool IsCorridor
        {
            get { return isCorridor; }
            set { isCorridor = value;}
        }

        public int WallCount
        {
            get
            {
                int wallCount = 0;
                if (northSide == SideType.Wall) wallCount++;
                if (southSide == SideType.Wall) wallCount++;
                if (westSide == SideType.Wall) wallCount++;
                if (eastSide == SideType.Wall) wallCount++;
                return wallCount;
            }
        }

        #endregion

        #region Methods

        public DirectionType CalculateDeadEndCorridorDirection()
        {
            if (!IsDeadEnd) throw new InvalidOperationException();

            if (northSide == SideType.Empty) return DirectionType.North;
            if (southSide == SideType.Empty) return DirectionType.South;
            if (westSide == SideType.Empty) return DirectionType.West;
            if (eastSide == SideType.Empty) return DirectionType.East;

            throw new InvalidOperationException();
        }

        #endregion
    }
}