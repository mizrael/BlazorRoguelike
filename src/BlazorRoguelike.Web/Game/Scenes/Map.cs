using System;
using System.Drawing;
using BlazorRoguelike.Core.Utils;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class Map
    {
        private readonly DungeonGenerator.Dungeon _dungeon;
        private readonly TileInfo[,] _tiles;

        public Map(DungeonGenerator.Dungeon dungeon)
        {
            _dungeon = dungeon;
            var cells = _dungeon.ExpandToTiles(4);
            this.Rows = cells.GetLength(0);
            this.Cols = cells.GetLength(1);

            _tiles = new TileInfo[this.Rows, this.Cols];
            for(int row=0;row<this.Rows;row++)
            for(int col=0;col<this.Cols;col++){
                var cell = cells[row, col];
                _tiles[row, col] = new TileInfo(row, col, cell);
            }
        }

        public readonly int Rows;
        public readonly int Cols;

        public TileInfo GetRandomEmptyTile()
        {
            int count = 0;
            while (count++<10)
            {
                var row = MathUtils.Random.Next(this.Rows);
                var col = MathUtils.Random.Next(this.Cols);
                var tile = GetTileAt(row, col);
                if (tile.Type == DungeonGenerator.TileType.Empty)
                    return tile;
            }
            return TileInfo.Void;
        }

        public TileInfo GetTileAt(int row, int col)
        {
            if (row < 0 || row > this.Rows - 1 || col < 0 || col > this.Cols - 1)
                return TileInfo.Void;
            return _tiles[row, col];
        }

    }

    public record TileInfo(int Row, int Col, DungeonGenerator.TileType Type)
    {
        public bool IsWalkable => this.Type == DungeonGenerator.TileType.Door || this.Type == DungeonGenerator.TileType.Empty;
       public static readonly TileInfo Void = new TileInfo(-1, -1, DungeonGenerator.TileType.Void);
    }
}
