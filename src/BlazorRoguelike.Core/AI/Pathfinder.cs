using System;
using System.Collections.Generic;
using System.Linq;
using BlazorRoguelike.Core.Utils;

namespace BlazorRoguelike.Core.AI
{
    /// Based on the implementation by Eric Lippert
    /// http://blogs.msdn.com/b/ericlippert/archive/2007/10/02/path-finding-using-a-in-c-3-0.aspx
    public class Pathfinder
    {
        public static Path<TN> FindPath<TN>(TN start,
                                            TN destination,
                                            Func<TN, TN, double> distance,
                                            Func<TN, TN, double> estimate,
                                            Func<TN, IEnumerable<TN>> findNeighbours)
        {
            var closed = new HashSet<TN>();
            var queue = new PriorityQueue<double, Path<TN>>();
            queue.Enqueue(0, new Path<TN>(start));
            while (!queue.IsEmpty)
            {
                var path = queue.Dequeue();
                if (closed.Contains(path.LastStep))
                    continue;
                if (path.LastStep.Equals(destination))
                    return path;
                closed.Add(path.LastStep);

                var neighs = findNeighbours(path.LastStep);
                if (null != neighs && neighs.Any())
                {
                    foreach (TN n in neighs)
                    {
                        double d = distance(path.LastStep, n);
                        var newPath = path.AddStep(n, d);
                        queue.Enqueue(newPath.TotalCost + estimate(n, destination), newPath);
                    }
                }
            }
            return null;
        }
    }
}