using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;
using System.Threading.Tasks;
using BlazorRoguelike.Web.Game.Components;
using System.Collections.Generic;
using BlazorRoguelike.Web.Game.Scenes;
using BlazorRoguelike.Core.Utils;
using System.Numerics;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PlayerBrain : Component
    {
        private TransformComponent _transform;
        private InputService _inputService;
        private Queue<TileInfo> _path;
        private TileInfo _currPathNode;

        private PlayerBrain(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask Init(GameContext game)
        {
            _transform = this.Owner.Components.Get<TransformComponent>();
            _inputService = game.GetService<InputService>();

            _inputService.Mouse.OnButtonStateChanged += (btn, state, oldState) =>
            {
                if (btn != MouseButtons.Left)
                    return;

                if (!state.IsClicked && oldState.IsClicked)
                {
                    var startTile = MapRenderer.GetTileAt(_transform.Local.Position);
                    var endTile = MapRenderer.GetTileAt(_inputService.Mouse.X, _inputService.Mouse.Y);
                    _path = MapRenderer.Map.FindPath(startTile, endTile);
                }
            };

            return ValueTask.CompletedTask;
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
            if (_path is null || (0 == _path.Count && _currPathNode is null))
                return;
            _currPathNode = _currPathNode ?? _path.Dequeue();

            var tilePos = MapRenderer.GetTilePos(_currPathNode);
            var newPos = Vector2Utils.MoveTowards(_transform.World.Position, tilePos, Speed);
            _transform.Local.Position = newPos;

            var dist = Vector2.DistanceSquared(newPos, tilePos);
            if (dist < Speed)
            {
                _currPathNode = null;                
                _transform.Local.Position = tilePos;
            }
        }

        public MapRenderComponent MapRenderer;
        public float Speed = 1.5f;
    }
}