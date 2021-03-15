using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerBrain : Component
    {
        private PathFollower _pathFollower;
        private InputService _inputService;
        private MapRenderComponent _mapRenderer;
        private GameObject _movementCursor;

        private PlayerBrain(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask Init(GameContext game)
        {
            var map = game.SceneManager.Current.FindGameObjectByName(ObjectNames.Map);
            _mapRenderer = map.Components.Get<MapRenderComponent>();

            _movementCursor = game.SceneManager.Current.FindGameObjectByName(ObjectNames.MovementCursor);
            
            _pathFollower = this.Owner.Components.Get<PathFollower>();
            _pathFollower.OnStartWalking += (_, from, to) =>
            {
                var tilePos = _mapRenderer.GetTilePos(to);
                _movementCursor.Components.Get<TransformComponent>().Local.Position = tilePos;
                _movementCursor.Enabled = true;
            };
            _pathFollower.OnArrived += _ =>
            {
                _movementCursor.Enabled = false;
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
                    _pathFollower.SetDestination(destination);
                }
            };

            return ValueTask.CompletedTask;
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
        }
    }
}