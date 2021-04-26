using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BlazorRoguelike.Core.Components;

namespace BlazorRoguelike.Core.GameServices
{

    public class CollisionService : IGameService
    {
        private readonly GameContext _game;

        private CollisionBucket[,] _buckets;
        private readonly Size _bucketSize;
        private readonly Dictionary<int, IList<CollisionBucket>> _bucketsByCollider = new();

        private readonly Queue<BoundingBoxComponent> _toAdd = new();

        public CollisionService(GameContext game, Size bucketSize)
        {
            _game = game;
            _bucketSize = bucketSize;
            _game.Display.OnSizeChanged += BuildBuckets;
            _game.SceneManager.OnSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene currentScene) => BuildBuckets();

        private void BuildBuckets()
        {
            var rows = _game.Display.Size.Height / _bucketSize.Height;
            var cols = _game.Display.Size.Width / _bucketSize.Width;
            _buckets = new CollisionBucket[rows, cols];

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    var bounds = new Rectangle(
                        col * _bucketSize.Width,
                        row * _bucketSize.Height,
                        _bucketSize.Width,
                        _bucketSize.Height);
                    _buckets[row, col] = new CollisionBucket(bounds);
                }

            _bucketsByCollider.Clear();

            var colliders = FindAllColliders();
            foreach (var collider in colliders)
            {
                RegisterCollider(collider);
            }
        }

        private void CheckCollisions(BoundingBoxComponent bbox)
        {
            RefreshColliderBuckets(bbox);

            var buckets = _bucketsByCollider[bbox.Owner.Id];
            foreach (var bucket in buckets)
            {
                bucket.CheckCollisions(bbox);
            }
        }

        private void RefreshColliderBuckets(BoundingBoxComponent collider)
        {
            var rows = _buckets.GetLength(0);
            var cols = _buckets.GetLength(1);
            var startX = (int)(cols * ((float)collider.Bounds.Left / _game.Display.Size.Width));
            var startY = (int)(rows * ((float)collider.Bounds.Top / _game.Display.Size.Height));

            var endX = (int)(cols * ((float)collider.Bounds.Right / _game.Display.Size.Width));
            var endY = (int)(rows * ((float)collider.Bounds.Bottom / _game.Display.Size.Height));

            if (!_bucketsByCollider.ContainsKey(collider.Owner.Id))
                _bucketsByCollider[collider.Owner.Id] = new List<CollisionBucket>();
            foreach (var bucket in _bucketsByCollider[collider.Owner.Id])
                bucket.Remove(collider);
            _bucketsByCollider[collider.Owner.Id].Clear();

            for (int row = startY; row <= endY; row++)
                for (int col = startX; col <= endX; col++)
                {
                    if (row < 0 || row >= rows)
                        continue;
                    if (col < 0 || col >= cols)
                        continue;

                    if (_buckets[row, col].Bounds.IntersectsWith(collider.Bounds))
                    {
                        _bucketsByCollider[collider.Owner.Id].Add(_buckets[row, col]);
                        _buckets[row, col].Add(collider);
                    }
                }
        }

        private IEnumerable<BoundingBoxComponent> FindAllColliders()
        {
            var scenegraph = _game.SceneManager.Current;
            var colliders = new List<BoundingBoxComponent>();

            FindAllColliders(scenegraph.Root, colliders);

            return colliders;
        }

        private void FindAllColliders(GameObject node, IList<BoundingBoxComponent> colliders)
        {
            if (node is null)
                return;

            if (node.Components.TryGet<BoundingBoxComponent>(out var bbox))
                colliders.Add(bbox);

            if (node.Children is not null)
                foreach (var child in node.Children)
                    FindAllColliders(child, colliders);
        }

        public IEnumerable<BoundingBoxComponent> FindColliders(int x, int y)
        {
            var rows = _buckets.GetLength(0);
            var cols = _buckets.GetLength(1);
            var col = (int)(cols * ((float)x / _game.Display.Size.Width));
            var row = (int)(rows * ((float)y / _game.Display.Size.Height));

            return (row >= 0 && row < rows && col >= 0 && col < cols) ?
                _buckets[row, col].Colliders.Where(c => c.Bounds.Contains(x, y)) : Enumerable.Empty<BoundingBoxComponent>();
        }

        public ValueTask Step()
        {
            if (null == _buckets)
                BuildBuckets();

            while (_toAdd.Any())
            {
                var collider = _toAdd.Dequeue();
                collider.OnPositionChanged -= CheckCollisions;
                if (!collider.IsStatic)
                    collider.OnPositionChanged += CheckCollisions;

                RefreshColliderBuckets(collider);
            }

            return ValueTask.CompletedTask;
        }

        public void RegisterCollider(BoundingBoxComponent collider)
        {
            _toAdd.Enqueue(collider);         
        }
    }
}