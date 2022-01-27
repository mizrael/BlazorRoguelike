using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI.FSM;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using BlazorRoguelike.Web.Game.AI;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerBrainComponent : FSMBrainComponent
    {
        private InputService _inputService;
        private MapRenderComponent _mapRenderer;
        private GameObject _movementCursor;

        private PlayerBrainComponent(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask Init(GameContext game)
        {
            var map = game.SceneManager.Current.FindGameObjectByName(ObjectNames.Map);
            _mapRenderer = map.Components.Get<MapRenderComponent>();

            _movementCursor = game.SceneManager.Current.FindGameObjectByName(ObjectNames.MovementCursor);

            var pathFollower = this.Owner.Components.Get<PathFollowerComponent>();
            pathFollower.OnStartWalking += (_, from, to) =>
            {
                var tilePos = _mapRenderer.GetTilePos(to);
                _movementCursor.Components.Get<TransformComponent>().Local.Position = tilePos;
                _movementCursor.Enabled = true;
            };
            pathFollower.OnArrived += _ =>
            {
                _movementCursor.Enabled = false;

                var newState = new AI.States.Idle(this.Owner, 0f);
                base.SetState(game, newState);
            };

            _inputService = game.GetService<InputService>();

            _inputService.Mouse.OnButtonStateChanged += (btn, state, oldState) =>
            {
                if (btn != MouseButtons.Left)
                    return;

                if (!state.IsClicked && oldState.IsClicked)
                {
                    _movementCursor.Enabled = false;

                    var destination = _mapRenderer.GetTileAt(_inputService.Mouse.X, _inputService.Mouse.Y);
                    if (!destination.IsWalkable)
                        return;

                    var newState = new AI.States.Arrive(this.Owner);
                    newState.SetDestination(destination);
                    base.SetState(game, newState);
                }
            };

            return ValueTask.CompletedTask;
        }
    }
}