using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;
using BlazorRoguelike.Web.Game.Components;
using BlazorRoguelike.Web.Game.Mechanics;

namespace BlazorRoguelike.Web.Game.AI.States
{
    public class Arrive : State
    {
        private PathFollowerComponent _pathFollower;

        public Arrive(GameObject owner) : base(owner)
        {
            _pathFollower = this.Owner.Components.Get<PathFollowerComponent>();
            _pathFollower.OnArrived += _ => {
                this.IsCompleted = true;
            };            
        }

        public void SetDestination(TileInfo dest)
            => _pathFollower.FindPathTo(dest);
    }
}