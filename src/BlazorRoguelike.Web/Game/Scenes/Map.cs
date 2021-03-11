using System;
using BlazorRoguelike.Core.Utils;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class Map
    {
        private readonly DungeonGenerator.Dungeon _dungeon;
        private readonly DungeonGenerator.TileType[,] _cells;

        public Map(DungeonGenerator.Dungeon dungeon)
        {
            _dungeon = dungeon;
            _cells = _dungeon.ExpandToTiles(4);
            this.Rows = _cells.GetLength(0);
            this.Cols = _cells.GetLength(1);            
        }

        public readonly int Rows;
        public readonly int Cols;

        public (int row, int col) GetRandomWalkableTile(){
            while(true){
                var row = MathUtils.Random.Next(this.Rows);
                var col = MathUtils.Random.Next(this.Cols);
                var cell = GetCellAt(row, col);
                if(cell == DungeonGenerator.TileType.Empty)
                    return (row, col);
            }            
        }

        public DungeonGenerator.TileType GetCellAt(int row, int col)
        {
            if (row < 0 || row > this.Rows - 1 || col < 0 || col > this.Cols - 1)
                return DungeonGenerator.TileType.Void;
            return _cells[row, col];
        }

        public bool IsWalkable(int row, int col){
            var cell = GetCellAt(row, col);
            return cell == DungeonGenerator.TileType.Empty || cell == DungeonGenerator.TileType.Door;
        }
    }
}
