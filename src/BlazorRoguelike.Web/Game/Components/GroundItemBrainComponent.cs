using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Web.Game.Mechanics;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Components
{
    public class GroundItemBrainComponent : Component
    {
        private GroundItemBrainComponent(GameObject owner) : base(owner)
        {
        }

        public MapObject Item { get; set; }

        protected override ValueTask Init(GameContext game)
        {
            var boundingBox = Owner.Components.Get<BoundingBoxComponent>();
            boundingBox.OnCollision += (sender, collidedWith) =>
            {
                if (!collidedWith.Owner.Components.TryGet<PlayerBrainComponent>(out var _))
                    return;

                this.Owner.Enabled = false;
                this.Owner.Parent.RemoveChild(this.Owner);

                if (collidedWith.Owner.Components.TryGet<InventoryComponent>(out var inventory))                    
                    inventory.Add(this.Item);
            };

            return base.Init(game);
        }
    }
}