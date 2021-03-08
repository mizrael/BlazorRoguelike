using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;

namespace BlazorRoguelike.Web.Game.Components
{
    public class DebugStatsUIComponent : Component, IRenderable
    {
        private const int startY = 50;
        private int y = 50;
        private const int _lineHeight = 30;
        private int x = 20;
        private int _height = _lineHeight * 5 + startY/2;

        private DebugStatsUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            var fps = 1000f / game.GameTime.ElapsedMilliseconds;

            await context.SetFillStyleAsync("green");
            await context.FillRectAsync(10, 50, 300, _height);

            await context.SetFillStyleAsync("#fff");
            await context.SetFontAsync("18px verdana");
            
            y = startY;

            await WriteLine($"Total game time (s): {game.GameTime.TotalMilliseconds / 1000}", context);
            await WriteLine($"Frame time (ms): {game.GameTime.ElapsedMilliseconds}", context);
            await WriteLine($"FPS: {fps:###}", context);
        }

        private async ValueTask WriteLine(string text, Canvas2DContext context)
        {
            y += _lineHeight;
            await context.FillTextAsync(text, x, y).ConfigureAwait(false);
        }
        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }
    }
}