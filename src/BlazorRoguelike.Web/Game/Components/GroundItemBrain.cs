using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Components
{
    public class GroundItemBrain : Component
    {
        private GroundItemBrain(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask Init(GameContext game)
        {
            var boundingBox = Owner.Components.Get<BoundingBoxComponent>();
            boundingBox.OnCollision += (sender, collidedWith) =>
            {
             //   if (!collidedWith.Owner.Components.TryGet<PlayerBrain>(out var _))
            //        return;

                this.Owner.Enabled = false;
                this.Owner.Parent.RemoveChild(this.Owner);
            };

            return base.Init(game);
        }
    }
}