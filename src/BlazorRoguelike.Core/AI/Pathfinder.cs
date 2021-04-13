using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRoguelike.Core.Utils;

namespace BlazorRoguelike.Core.AI
{
    /// Based on the implementation by Eric Lippert
    /// http://blogs.msdn.com/b/ericlippert/archive/2007/10/02/path-finding-using-a-in-c-3-0.aspx
    public class Pathfinder
    {
        private class TempPath<TN> : IEnumerable<TN>
        {
            public TN LastStep { get; private set; }
            public TempPath<TN> PreviousSteps { get; private set; }
            public double TotalCost { get; private set; }

            private TempPath(TN lastStep, TempPath<TN> previousSteps, double totalCost)
            {
                LastStep = lastStep;
                PreviousSteps = previousSteps;
                TotalCost = totalCost;
            }
            public TempPath(TN start) : this(start, null, 0) { }
            public TempPath<TN> AddStep(TN step, double stepCost)
            {
                return new TempPath<TN>(step, this, TotalCost + stepCost);
            }

            public IEnumerator<TN> GetEnumerator()
            {
                for (TempPath<TN> p = this; p != null; p = p.PreviousSteps)
                    yield return p.LastStep;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// https://github.com/dotnet/aspnetcore/issues/17730
        /// </summary>
        public static Task<Path<TN>> FindPathAsync<TN>(TN start,
                                            TN destination,
                                            Func<TN, TN, double> distance,
                                            Func<TN, TN, double> estimate,
                                            Func<TN, IEnumerable<TN>> findNeighbours)
            => Task.Run(() => FindPathCore(start, destination, distance, estimate, findNeighbours));

        private static Path<TN> FindPathCore<TN>(TN start, TN destination, Func<TN, TN, double> distance, Func<TN, TN, double> estimate, Func<TN, IEnumerable<TN>> findNeighbours)
        {
            var path = RunPathfinder(start, destination, distance, estimate, findNeighbours);
            if (path is null)
                return Path<TN>.Empty;
            
            var steps = path.Reverse();
            return new Path<TN>(steps);
        }

        private static TempPath<TN> RunPathfinder<TN>(TN start,
                                            TN destination,
                                            Func<TN, TN, double> distance,
                                            Func<TN, TN, double> estimate,
                                            Func<TN, IEnumerable<TN>> findNeighbours)
        {
            var closed = new HashSet<TN>();
            var queue = new PriorityQueue<double, TempPath<TN>>();
            queue.Enqueue(0, new TempPath<TN>(start));
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