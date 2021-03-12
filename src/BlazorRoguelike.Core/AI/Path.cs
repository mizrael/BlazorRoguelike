using System.Collections.Generic;
using System.Linq;

namespace BlazorRoguelike.Core.AI
{
    public class Path<T>
    {
        private readonly Queue<T> _steps = new();
        public Path(IEnumerable<T> steps)
        {
            foreach (var node in steps)
                _steps.Enqueue(node);
        }
        
        public readonly static Path<T> Empty = new Path<T>(Enumerable.Empty<T>());

        public bool Any() => _steps.Any();
        public T Next() => _steps.Dequeue();
    }
}