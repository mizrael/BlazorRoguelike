using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.DungeonGenerator;

namespace BlazorRoguelike.Web.Game.Components
{
    public class MapRenderComponent : Component, IRenderable
    {
        private MapRenderComponent(GameObject owner) : base(owner)
        {
        }

        public OffscreenMapRenderer Renderer;

        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            await this.Renderer.Render();

            await context.DrawImageAsync(this.Renderer.Canvas.Canvas, 0, 0);
        }
    }

    public class OffscreenMapRenderer
    {
        private Dungeon _dungeon;
        private TileType[,] _cells;
        private int _tileWidth = 16;
        private int _tileHeigth = 16;
        private Core.Assets.SpriteSheet _tileset;
        private Canvas2DContext _canvas;
        private bool _canRender = true;

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

        public async ValueTask Render()
        {
            if (_cells is null || !_canRender)
                return;
                System.Console.WriteLine("map");

            _canRender = false;

            int rows = _cells.GetLength(0),
                cols = _cells.GetLength(1);

            await this.Canvas.ClearRectAsync(0, 0, TileWidth * rows, TileHeight * cols)
                        .ConfigureAwait(false);

            await this.Canvas.BeginBatchAsync().ConfigureAwait(false);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var cell = _cells[row, col];

                    var tile = Tileset.GetSprite(_tileNames[cell]);
                    if (tile is null)
                        continue;

                    await this.Canvas.DrawImageAsync(tile.ElementRef,
                        tile.Bounds.X, tile.Bounds.Y, tile.Bounds.Width, tile.Bounds.Height,
                        row * TileWidth, col * TileHeight,
                        TileWidth, TileHeight).ConfigureAwait(false);
                }
            }

            await this.Canvas.EndBatchAsync().ConfigureAwait(false);
        }

        public void ForceRendering() => _canRender = true;

        public Dungeon Dungeon
        {
            get => _dungeon;
            set
            {
                _dungeon = value;
                _cells = _dungeon.ExpandToTiles(4);
                _canRender = true;
            }
        }
        public int TileWidth
        {
            get => _tileWidth;
            set
            {
                _tileWidth = value;
                _canRender = true;
            }
        }
        public int TileHeight
        {
            get => _tileHeigth;
            set
            {
                _tileHeigth = value;
                _canRender = true;
            }
        }

        public Canvas2DContext Canvas
        {
            get => _canvas;
            set
            {
                _canvas = value;
                _canRender = true;
            }
        }

        public Core.Assets.SpriteSheet Tileset
        {
            get => _tileset;
            set
            {
                _tileset = value;
                _canRender = true;
            }
        }
    }
}