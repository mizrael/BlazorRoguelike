using System.Numerics;
using System.Threading.Tasks;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.AI;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.Utils;
using BlazorRoguelike.Web.Game.Mechanics;

namespace BlazorRoguelike.Web.Game.Components
{
    public class PathFollowerComponent : Component
    {
        private Path<TileInfo> _path;
        private TileInfo _currPathNode;
        private TransformComponent _transform;
        private MapRenderComponent _mapRenderer;

        public float Speed = 1.5f; //TODO: create StatsComponent

        public PathFollowerComponent(GameObject owner) : base(owner)
        {
        }

        protected override async ValueTask Init(GameContext game)
        {
            _transform = this.Owner.Components.Get<TransformComponent>();

            var map = game.SceneManager.Current.FindGameObjectByName(ObjectNames.Map);
            _mapRenderer = map.Components.Get<MapRenderComponent>();
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
            if (_path is null || (!_path.Any() && _currPathNode is null))
                return;

            _currPathNode ??= _path.Next();

            var tilePos = _mapRenderer.GetTilePos(_currPathNode);
            var newPos = Vector2Utils.MoveTowards(_transform.World.Position, tilePos, Speed);
            _transform.Local.Position = newPos;

            var dist = Vector2.DistanceSquared(newPos, tilePos);
            if (dist < Speed)
            {
                _currPathNode = null;
                _transform.Local.Position = tilePos;

                if (!_path.Any())
                {
                    _path = null;
                    OnArrived?.Invoke(this);
                }
            }
        }

        public void FindPathTo(TileInfo destination)
        {
            _currPathNode = null;
            _path = null;

            if (null == destination)
                return;

            var startTile = _mapRenderer.GetTileAt(_transform.Local.Position);
            Task.Run(() => _mapRenderer.Map.FindPathAsync(startTile, destination)
                .ContinueWith(t =>
                {
                    _path = t.Result;
                    if (_path.Any())
                        this.OnStartWalking?.Invoke(this, startTile, destination);
                }).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        public event OnStartWalkingHandler OnStartWalking;
        public delegate void OnStartWalkingHandler(PathFollowerComponent sender, TileInfo from, TileInfo to);

        public event OnArrivedHandler OnArrived;
        public delegate void OnArrivedHandler(PathFollowerComponent sender);
    }
}