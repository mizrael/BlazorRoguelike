using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Components;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerUIComponent : Component, IRenderable
    {
        const int itemMarginX = 10;
        const int rightMargin = 10;
        const int heartsTopMargin = 20;
        const int potionsTopMargin = 40;

        private PlayerStatsComponent _playerStatsComponent;
        private PlayerInventoryComponent _playerInventoryComponent;

        private PlayerUIComponent(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask Init(GameContext game)
        {
            _playerStatsComponent = this.Player.Components.Get<PlayerStatsComponent>();
            _playerInventoryComponent = this.Player.Components.Get<PlayerInventoryComponent>();

            return base.Init(game);
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            if (!this.Owner.Enabled || this.Hidden)
                return;

            await RenderHearts(game, context);
            await RenderPotions(game, context);
        }

        private async ValueTask RenderPotions(GameContext game, Canvas2DContext context)
        {
            var startX = game.Display.Size.Width - rightMargin;
            var x = PotionSprite.Bounds.Width + itemMarginX;

            await context.SaveAsync();            
            await context.TranslateAsync(startX, potionsTopMargin);

            for (int i = 0; i < _playerInventoryComponent.Potions; i++)
                await RenderSprite(context, -x, PotionSprite);

            await context.RestoreAsync();
        }

        private async ValueTask RenderHearts(GameContext game, Canvas2DContext context)
        {            
            int statesCount = this.HeartSprites.Length - 1;
            int fullHeartsCount = (int)System.MathF.Floor((float)_playerStatsComponent.Health / statesCount);
            
            int diff = _playerStatsComponent.MaxHealth - _playerStatsComponent.Health;
            float div = (float)diff / statesCount;
            float diff2 = System.MathF.Ceiling(div) - div;
            int halfHeartsCount = (int)System.MathF.Ceiling(diff2);
            int emptyHeartsCount = (int)System.MathF.Floor(div);
                        
            var startX = game.Display.Size.Width - rightMargin;            
            var x = HeartSprites[0].Bounds.Width + itemMarginX;

            await context.SaveAsync();
            await context.TranslateAsync(startX, heartsTopMargin);            

            for (int i = 0; i < fullHeartsCount; i++)                         
                await RenderSprite(context, -x, HeartSprites[2]);
            for (int i = 0; i < halfHeartsCount; i++)
                await RenderSprite(context, -x, HeartSprites[1]);
            for (int i = 0; i < emptyHeartsCount; i++)
                await RenderSprite(context, -x, HeartSprites[0]);

            await context.RestoreAsync();
        }

        private static async Task RenderSprite(Canvas2DContext context, int x, SpriteBase sprite)
        {
            await context.TranslateAsync(x, 0);
            await context.DrawImageAsync(sprite.ElementRef,
                sprite.Bounds.X, sprite.Bounds.Y,
                sprite.Bounds.Width, sprite.Bounds.Height,
                sprite.Origin.X, sprite.Origin.Y,
                -sprite.Bounds.Width, -sprite.Bounds.Height);
        }

        public int LayerIndex { get; set; } = (int)RenderLayers.UI;
        public bool Hidden { get; set; } = false;
        public GameObject Player { get; set; }   
        public SpriteBase[] HeartSprites { get; set; }
        public SpriteBase PotionSprite { get; set; }
    }
}