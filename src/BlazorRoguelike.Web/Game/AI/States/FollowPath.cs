using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;
using BlazorRoguelike.Web.Game.Components;

namespace BlazorRoguelike.Web.Game.AI.States
{
    public class FollowPath : State
    {
        private PathFollower _pathFollower;

        public FollowPath(GameObject owner, TileInfo dest) : base(owner)
        {
            _pathFollower = this.Owner.Components.Get<PathFollower>();
            _pathFollower.OnArrived += _ =>{
                this.Completed = true;
            };
            _pathFollower.SetDestination(dest);
        }

    }
}