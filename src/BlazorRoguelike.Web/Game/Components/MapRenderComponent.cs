using System.Numerics;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;

namespace BlazorRoguelike.Web.Game.Components
{
    public class MapRenderComponent : Component, IRenderable
    {
        private MapRenderComponent(GameObject owner) : base(owner)
        {
        }

        public OffscreenMapRenderer OffscreenRenderer;
        public GameObject Player;

        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }

        public Map Map => OffscreenRenderer?.Map;

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            await this.OffscreenRenderer.Render();

            await context.DrawImageAsync(this.OffscreenRenderer.Image, 0, 0).ConfigureAwait(false);
        }

        public TileInfo GetTileAt(Vector2 point)
        {
            var x = (int)(point.X / OffscreenRenderer.TileWidth);
            var y = (int)(point.Y / OffscreenRenderer.TileHeight);
            return OffscreenRenderer.Map.GetTileAt(x, y);
        }

        public TileInfo GetTileAt(int screenX, int screenY)
        {
            var x = (int)((float)screenX / OffscreenRenderer.TileWidth);
            var y = (int)((float)screenY / OffscreenRenderer.TileHeight);
            return OffscreenRenderer.Map.GetTileAt(x, y);
        }

        public Vector2 GetTilePos(TileInfo tile)
        {
            return new Vector2(
                tile.Row * OffscreenRenderer.TileWidth + OffscreenRenderer.TileWidth / 2,
                tile.Col * OffscreenRenderer.TileHeight + OffscreenRenderer.TileHeight / 2
            );
        }

    }
}