using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;

namespace BlazorRoguelike.Core.Components
{
    public class BoundingBoxComponent : Component
#if DEBUG
    //    , IRenderable
#endif
    {
        private TransformComponent _transform;
        private Rectangle _bounds;
        private Size _halfSize;

        private BoundingBoxComponent(GameObject owner) : base(owner)
        {

        }

        protected override ValueTask Init(GameContext game)
        {
            _transform = Owner.Components.Get<TransformComponent>();
            return ValueTask.CompletedTask;
        }

        public Rectangle Bounds => _bounds;

        public void SetSize(Size size)
        {
            _bounds.Size = size;
            _halfSize = size / 2;
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
            var x = (int)_transform.World.Position.X - _halfSize.Width;
            var y = (int)_transform.World.Position.Y - _halfSize.Height;

            var changed = _bounds.X != x || _bounds.Y != y;
            _bounds.X = x;
            _bounds.Y = y;

            if (changed)
                OnPositionChanged?.Invoke(this);
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            var tmpW = context.LineWidth;
            var tmpS = context.StrokeStyle;

            await context.BeginPathAsync();
            await context.SetStrokeStyleAsync("rgb(255,255,0)");
            await context.SetLineWidthAsync(3);
            await context.StrokeRectAsync(_bounds.X, _bounds.Y,
                _bounds.Width,
                _bounds.Height);

            await context.SetStrokeStyleAsync(tmpS);
            await context.SetLineWidthAsync(tmpW);
        }

        public event OnPositionChangedHandler OnPositionChanged;
        public delegate void OnPositionChangedHandler(BoundingBoxComponent sender);

        public void CollideWith(BoundingBoxComponent other) => this.OnCollision?.Invoke(this, other);

        public event OnCollisionHandler OnCollision;
        public delegate void OnCollisionHandler(BoundingBoxComponent sender, BoundingBoxComponent collidedWith);
    }
}