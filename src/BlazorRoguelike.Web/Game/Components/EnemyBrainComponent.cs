using BlazorRoguelike.Core;
using BlazorRoguelike.Web.Game.Components;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Scenes
{
    public class EnemyBrainComponent : FSMBrainComponent
    {
        protected EnemyBrainComponent(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask Init(GameContext game)
        {
            var newState = new AI.States.Idle(this.Owner);
            base.SetState(newState);

            return ValueTask.CompletedTask;
        }
    }
}