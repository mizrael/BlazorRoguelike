using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorRoguelike.Web.Game.DungeonGenerator;
using Microsoft.AspNetCore.Components;

namespace BlazorRoguelike.Web.Game.Components
{
    public class OffscreenMapRenderer
    {
        private Mechanics.Map _map;
        private int _tileWidth = 16;
        private int _tileHeigth = 16;
        private int _mapWidth;
        private int _mapHeight;
        private readonly Core.Assets.SpriteSheet _tileset;
        private readonly Canvas2DContext _canvas;
        private bool _needRefresh = true;

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
        
        public OffscreenMapRenderer(Canvas2DContext canvasContext, Core.Assets.SpriteSheet tileset)
        {
            _canvas = canvasContext;            
            _tileset = tileset;         
        }

        public async ValueTask Render()
        {
            if (_map is null || !_needRefresh)
                return;

            _needRefresh = false;

            await _canvas.ClearRectAsync(0, 0, _mapWidth, _mapHeight)
                        .ConfigureAwait(false);

            await _canvas.BeginBatchAsync().ConfigureAwait(false);

            for (int row = 0; row < _map.Rows; row++)
            {
                for (int col = 0; col < _map.Cols; col++)
                {
                    var tile = _map.GetTileAt(row, col);

                    var tileSprite = _tileset.GetSprite(_tileNames[tile.Type]);
                    if (tileSprite is null)
                        continue;

                    await _canvas.DrawImageAsync(tileSprite.ElementRef,
                        tileSprite.Bounds.X, tileSprite.Bounds.Y, tileSprite.Bounds.Width, tileSprite.Bounds.Height,
                        row * TileWidth, col * TileHeight,
                        TileWidth, TileHeight).ConfigureAwait(false);
                }
            }

            await _canvas.EndBatchAsync().ConfigureAwait(false);
        }

        public void ForceRendering() => _needRefresh = true;

        public Mechanics.Map Map
        {
            get => _map;
            set
            {
                _map = value;

                _mapWidth = TileWidth * _map.Rows;
                _mapHeight = TileHeight * _map.Cols;

                _needRefresh = true;
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

                _needRefresh = true;
            }
        }        

        public int TileHeight
        {
            get => _tileHeigth;
            set
            {
                _tileHeigth = value;
                if (_map is not null)
                    _mapHeight = TileHeight * _map.Cols;
                _needRefresh = true;
            }
        }       

        public ElementReference Image { get => _canvas.Canvas; }
    }
}