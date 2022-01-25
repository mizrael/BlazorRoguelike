using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;
using BlazorRoguelike.Core.Components;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class EnemyBrainComponent : Component
    {
        private Machine _wanderAroundSpot;

        protected EnemyBrainComponent(GameObject owner) : base(owner)
        {            
        }

        protected override ValueTask Init(GameContext game)
        {
            _wanderAroundSpot = AI.StateMachines.WanderAroundSpot(this.Owner, game);

            return ValueTask.CompletedTask;
        }

        protected override ValueTask UpdateCore(GameContext game)
        {
            _wanderAroundSpot.Update(game);

            return base.UpdateCore(game);
        }
    }
}