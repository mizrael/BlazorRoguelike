using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Components;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerUIComponent : Component, IRenderable
    {
        private const int leftMargin = 20;
        private const int bottomMargin = 50;

        private PlayerStatsComponent _playerStatsComponent;
        private PlayerInventoryComponent _playerInventoryComponent;

        private SpriteBase _heartSprite;
        private SpriteBase _inventorySlotSprite;

        private SpriteSheet _spriteSheet;

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

            var y = game.Display.Size.Height - bottomMargin;

            await context.SaveAsync();
            await context.TranslateAsync(leftMargin, y);

            await RenderHearts(game, context);
            await RenderInventory(game, context);

            await context.RestoreAsync();
        }

        private async ValueTask RenderInventory(GameContext game, Canvas2DContext context)
        {
            //TODO: pre-render the slots

            await context.TranslateAsync(0, _inventorySlotSprite.Bounds.Height);
            for (int i = 0; i != PlayerInventoryComponent.MaxSlots; ++i)
            {
                await RenderSprite(context, _inventorySlotSprite);

                var item = _playerInventoryComponent.GetItemAt(i);
                if(item != null && 
                    item.TryGetProperty<string>("inventorySprite", out var itemSpriteName) &&
                    _spriteSheet.TryGetSprite(itemSpriteName, out var itemSprite))
                {
                    await RenderSprite(context, itemSprite);
                }

                await context.TranslateAsync(_inventorySlotSprite.Bounds.Width, 0);
            }
        }

        private async ValueTask RenderHearts(GameContext game, Canvas2DContext context)
        {            
            var text = $"{_playerStatsComponent.Health}/{_playerStatsComponent.MaxHealth}";

            await RenderSprite(context, _heartSprite);

            await RenderText(context, text, _heartSprite.Bounds.Width);
        }

        private static async Task RenderText(Canvas2DContext context, string text, int x)
        {
            await context.SetFillStyleAsync("#fff");
            await context.SetFontAsync("18px verdana");
            await context.SetTextBaselineAsync(TextBaseline.Middle);
            await context.FillTextAsync(text, x, 0);
        }

        private static async Task RenderSprite(Canvas2DContext context, SpriteBase sprite)
        {
            await context.DrawImageAsync(sprite.ElementRef,
                sprite.Bounds.X, sprite.Bounds.Y,
                sprite.Bounds.Width, sprite.Bounds.Height,
                sprite.Origin.X, sprite.Origin.Y,
                -sprite.Bounds.Width, -sprite.Bounds.Height);
        }

        public int LayerIndex { get; set; } = (int)RenderLayers.UI;
        public bool Hidden { get; set; } = false;
        public GameObject Player { get; set; }
        
        public void SetSpriteSheet(SpriteSheet spriteSheet)
        {
            _spriteSheet = spriteSheet;
            _inventorySlotSprite = spriteSheet.GetSprite("ui-inventory-slot");
            _heartSprite = spriteSheet.GetSprite("ui-heart3");
        }
    }
}