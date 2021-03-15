using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorRoguelike.Web.Game.DungeonGenerator;

namespace BlazorRoguelike.Web.Game.Components
{
    public class OffscreenMapRenderer
    {
        private Game.Map _map;
        private int _tileWidth = 16;
        private int _tileHeigth = 16;
        private int _mapWidth;
        private int _mapHeight;
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
            if (_map is null || !_canRender)
                return;

            _canRender = false;

            await this.Canvas.ClearRectAsync(0, 0, _mapWidth, _mapHeight)
                        .ConfigureAwait(false);

            await this.Canvas.BeginBatchAsync().ConfigureAwait(false);

            for (int row = 0; row < _map.Rows; row++)
            {
                for (int col = 0; col < _map.Cols; col++)
                {
                    var tile = _map.GetTileAt(row, col);

                    var tileSprite = Tileset.GetSprite(_tileNames[tile.Type]);
                    if (tileSprite is null)
                        continue;

                    await this.Canvas.DrawImageAsync(tileSprite.ElementRef,
                        tileSprite.Bounds.X, tileSprite.Bounds.Y, tileSprite.Bounds.Width, tileSprite.Bounds.Height,
                        row * TileWidth, col * TileHeight,
                        TileWidth, TileHeight).ConfigureAwait(false);
                }
            }

            await this.Canvas.EndBatchAsync().ConfigureAwait(false);
        }

        public void ForceRendering() => _canRender = true;

        public Game.Map Map
        {
            get => _map;
            set
            {
                _map = value;

                _mapWidth = TileWidth * _map.Rows;
                _mapHeight = TileHeight * _map.Cols;

                _canRender = true;
            }
        }

        public int TileWidth
        {
            get => _tileWidth;
            set
            {
                _tileWidth = value;
                if (_map is not null)
                    _mapWidth = TileWidth * _map.Rows;
                _canRender = true;
            }
        }
        public int MapWidth => _mapWidth;
        public int TileHeight
        {
            get => _tileHeigth;
            set
            {
                _tileHeigth = value;
                if (_map is not null)
                    _mapHeight = TileHeight * _map.Cols;
                _canRender = true;
            }
        }
        public int MapHeight => _mapHeight;

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