using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.DungeonGenerator;

namespace BlazorRoguelike.Web.Game.Components
{
    public class MapRenderer : Component, IRenderable
    {
        private TileType[,] _cells;
      
        private Dictionary<TileType, string> _tileNames = new()
        {
            { TileType.Empty, "floor" }, //
            { TileType.Void, "void" }, //
            { TileType.WallEO, "walleo" }, //
            { TileType.WallESO, "walleso" }, //
            { TileType.WallNE, "wallne" }, //
            { TileType.WallNEO, "wallneo" }, //
            { TileType.WallNES, "wallnes" }, //
            { TileType.WallNESO, "wallneso" },
            { TileType.WallNO, "wallno" }, //
            { TileType.WallNS, "wallns" }, //
            { TileType.WallNSO, "wallnso" }, //
            { TileType.WallSE, "wallse" }, //
            { TileType.WallSO, "wallso" }, //
            { TileType.Wall, "wall" },
            { TileType.Door, "door" }
        };

        public MapRenderer(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            if (this.Dungeon is null)
                return;

            for (int row = 0; row < _cells.GetLength(0); row++)
            {
                for (int col = 0; col < _cells.GetLength(1); col++)
                {
                    var cell = _cells[row, col];

                    var tile = Tileset.Get(_tileNames[cell]);
                    if(tile is null)
                        continue;

                    await context.DrawImageAsync(tile.ElementRef,
                        tile.Bounds.X, tile.Bounds.Y, tile.Bounds.Width, tile.Bounds.Height,
                        row * TileWidth, col * TileHeight,
                        TileWidth, TileHeight);
                }
            }
        }

        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }

        private Dungeon _dungeon;
        public Dungeon Dungeon
        {
            get => _dungeon;
            set
            {
                _dungeon = value;
                _cells = _dungeon.ExpandToTiles(3);
            }
        }
        public int TileWidth { get; set; } = 16;
        public int TileHeight { get; set; } = 16;

        public Core.Assets.SpriteSheet Tileset { get; set; }
    }
}