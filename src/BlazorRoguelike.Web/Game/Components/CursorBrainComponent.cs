using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using BlazorRoguelike.Web.Game.Components;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class CursorBrainComponent : Component
    {
        private InputService _inputService;
        private CollisionService _collisionService;
        private TransformComponent _transform;
        private MapRenderComponent _mapRenderer;
        private SpriteRenderComponent _renderer;

        protected CursorBrainComponent(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask Init(GameContext game)
        {
            _inputService = game.GetService<InputService>();
            _collisionService = game.GetService<CollisionService>();

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

            var colliders = _collisionService.FindColliders(_inputService.Mouse.X, _inputService.Mouse.Y);
            if (!colliders.Any())
            {
                var tile = _mapRenderer.GetTileAt(_transform.Local.Position);
                _renderer.Sprite = tile.IsWalkable ? WalkableSprite : ForbiddenSprite;
            }
            else
            {
                _renderer.Sprite = SelectionSprite;
            }

            return base.UpdateCore(game);
        }

        public SpriteBase WalkableSprite { get; set; }
        public SpriteBase ForbiddenSprite { get; set; }
        public SpriteBase SelectionSprite { get; set; }
    }
}