using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using BlazorRoguelike.Web.Game.Components;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class CursorBrainComponent : Component
    {
        private InputService _inputService;
        private TransformComponent _transform;
        private MapRenderComponent _mapRenderer;
        private SpriteRenderComponent _renderer;

        protected CursorBrainComponent(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask Init(GameContext game)
        {
            _inputService = game.GetService<InputService>();

            _transform = this.Owner.Components.Get<TransformComponent>();
            _renderer = this.Owner.Components.Get<SpriteRenderComponent>();

            var map = game.SceneManager.Current.FindGameObjectByName(ObjectNames.Map);
            _mapRenderer = map.Components.Get<MapRenderComponent>();

            return base.Init(game);
        }

        protected override ValueTask UpdateCore(GameContext game)
        {
            _transform.Local.Position.X = _inputService.Mouse.X;
            _transform.Local.Position.Y = _inputService.Mouse.Y;

            var tile = _mapRenderer.GetTileAt(_transform.Local.Position);
            _renderer.Sprite = tile.IsWalkable ? WalkableSprite : CursorXSprite;

            return base.UpdateCore(game);
        }

        public SpriteBase WalkableSprite { get; set; }
        public SpriteBase CursorXSprite { get; set; }
    }
}