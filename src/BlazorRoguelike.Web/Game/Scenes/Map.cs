using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BlazorRoguelike.Core.AI;
using BlazorRoguelike.Core.Utils;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class Map
    {
        private readonly DungeonGenerator.Dungeon _dungeon;
        private readonly TileInfo[,] _tiles;

        private Func<TileInfo, TileInfo, double> _distanceFunc;
        private Func<TileInfo, TileInfo, double> _estimateFunc;
        private Func<TileInfo, IEnumerable<TileInfo>> _findNeighboursFunc;

        public Map(DungeonGenerator.Dungeon dungeon)
        {
            _dungeon = dungeon;
            var cells = _dungeon.ExpandToTiles(4);
            this.Rows = cells.GetLength(0);
            this.Cols = cells.GetLength(1);

            _tiles = new TileInfo[this.Rows, this.Cols];
            for (int row = 0; row < this.Rows; row++)
                for (int col = 0; col < this.Cols; col++)
                {
                    var cell = cells[row, col];
                    _tiles[row, col] = new TileInfo(row, col, cell);
                }

            _findNeighboursFunc = t => GetNeighbours(t, n => null != n && n.IsWalkable);
            _distanceFunc = _estimateFunc = (t1, t2) =>
            {
                int dx = t2.Row - t1.Row;
                int dy = t2.Col - t1.Col;
                return dx * dx + dy * dy;
            };
        }

        public readonly int Rows;
        public readonly int Cols;

        public TileInfo GetRandomEmptyTile()
        {
            int count = 0;
            while (count++ < 10)
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

        public Path<TileInfo> FindPath(TileInfo start, TileInfo destination)
        {
            if (null == start || null == destination)
                return Path<TileInfo>.Empty;

            if (start == destination)            
                return new Path<TileInfo>(new[]{ destination});            

            return Pathfinder.FindPath(start, destination,
                           _distanceFunc,
                           _estimateFunc,
                           _findNeighboursFunc);          
        }
        
        public TileInfo[] GetNeighbours(TileInfo tile, Predicate<TileInfo> filter)
        {
            var results = new List<TileInfo>(8);

            int x = tile.Row - 1;
            int y = tile.Col - 1;
            if (x > -1 && x < this.Rows && y > -1 && y < this.Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row;
            y = tile.Col - 1;
            if (x > -1 && x < this.Rows && y > -1 && y < this.Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row + 1;
            y = tile.Col - 1;
            if (x > -1 && x < this.Rows && y > -1 && y < this.Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row - 1;
            y = tile.Col;
            if (x > -1 && x < this.Rows && y > -1 && y < this.Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row + 1;
            y = tile.Col;
            if (x > -1 && x < this.Rows && y > -1 && y < this.Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row - 1;
            y = tile.Col + 1;
            if (x > -1 && x < this.Rows && y > -1 && y < this.Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row;
            y = tile.Col + 1;
            if (x > -1 && x < this.Rows && y > -1 && y < this.Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row + 1;
            y = tile.Col + 1;
            if (x > -1 && x < this.Rows && y > -1 && y < this.Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            return results.ToArray();
        }
    }

    public record TileInfo(int Row, int Col, DungeonGenerator.TileType Type)
    {
        public bool IsWalkable => this.Type == DungeonGenerator.TileType.Door || this.Type == DungeonGenerator.TileType.Empty;
        public static readonly TileInfo Void = new TileInfo(-1, -1, DungeonGenerator.TileType.Void);
    }
}
