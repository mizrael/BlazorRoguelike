using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.Mechanics;
using System;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Components
{
    public class GroundItemBrainComponent : Component
    {
        private TransformComponent _transform;

        private GroundItemBrainComponent(GameObject owner) : base(owner)
        {
        }

        public MapObject Item { get; set; }

        protected override ValueTask Init(GameContext game)
        {
            var boundingBox = Owner.Components.Get<BoundingBoxComponent>();
            boundingBox.OnCollision += (sender, collidedWith) =>
            {
                if (!collidedWith.Owner.Components.TryGet<PlayerInventoryComponent>(out var inventory) ||
                    !inventory.CanAdd(this.Item))   
                    return;

                this.Owner.Enabled = false;
                this.Owner.Parent.RemoveChild(this.Owner);

                inventory.Add(this.Item);                
            };

            _transform = Owner.Components.Get<TransformComponent>();

            return base.Init(game);
        }

        protected override ValueTask UpdateCore(GameContext game)
        {
            _transform.Local.Position.Y += MathF.Sin(game.GameTime.TotalMilliseconds * 0.005f) * 0.25f;

            return base.UpdateCore(game);
        }
    }
}