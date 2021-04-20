using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorRoguelike.Core.AI;
using BlazorRoguelike.Core.Utils;

namespace BlazorRoguelike.Web.Game.Mechanics
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
            Rows = cells.GetLength(0);
            Cols = cells.GetLength(1);

            _tiles = new TileInfo[Rows, Cols];
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Cols; col++)
                {
                    var cell = cells[row, col];
                    _tiles[row, col] = new TileInfo(row, col, cell);
                }

            _findNeighboursFunc = t => GetNeighbours(t, n => null != n && n.IsWalkable);
            _distanceFunc = _estimateFunc = (t1, t2) =>
            {
                int dx = t2.Row - t1.Row;
                int dy = t2.Col - t1.Col;
                return Math.Sqrt(dx * dx + dy * dy);
            };
        }

        public readonly int Rows;
        public readonly int Cols;

        public TileInfo GetRandomEmptyTile()
        {
            int count = 0;
            while (count++ < 10)
            {
                var row = MathUtils.Random.Next(Rows);
                var col = MathUtils.Random.Next(Cols);
                var tile = GetTileAt(row, col);
                if (tile.Type == DungeonGenerator.TileType.Empty)
                    return tile;
            }
            return TileInfo.Void;
        }

        public TileInfo GetTileAt(int row, int col)
        {
            if (row < 0 || row > Rows - 1 || col < 0 || col > Cols - 1)
                return TileInfo.Void;
            return _tiles[row, col];
        }

        public async Task<Path<TileInfo>> FindPathAsync(TileInfo start, TileInfo destination)
        {
            if (null == start || null == destination)
                return Path<TileInfo>.Empty;

            if (start == destination)
                return new Path<TileInfo>(new[] { destination });

            return await Pathfinder.FindPathAsync(start, destination,
                                                   _distanceFunc,
                                                   _estimateFunc,
                                                   _findNeighboursFunc)
                            .ConfigureAwait(false);
        }

        public TileInfo[] GetNeighbours(TileInfo tile, Predicate<TileInfo> filter)
        {
            var results = new List<TileInfo>(8);

            int x = tile.Row - 1;
            int y = tile.Col - 1;
            if (x > -1 && x < Rows && y > -1 && y < Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row;
            y = tile.Col - 1;
            if (x > -1 && x < Rows && y > -1 && y < Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row + 1;
            y = tile.Col - 1;
            if (x > -1 && x < Rows && y > -1 && y < Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row - 1;
            y = tile.Col;
            if (x > -1 && x < Rows && y > -1 && y < Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row + 1;
            y = tile.Col;
            if (x > -1 && x < Rows && y > -1 && y < Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row - 1;
            y = tile.Col + 1;
            if (x > -1 && x < Rows && y > -1 && y < Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row;
            y = tile.Col + 1;
            if (x > -1 && x < Rows && y > -1 && y < Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            x = tile.Row + 1;
            y = tile.Col + 1;
            if (x > -1 && x < Rows && y > -1 && y < Cols && filter(_tiles[x, y]))
                results.Add(_tiles[x, y]);

            return results.ToArray();
        }
    }
}
