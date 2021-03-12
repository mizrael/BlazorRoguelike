using System.Collections;
using System.Collections.Generic;

namespace BlazorRoguelike.Core.AI
{
    public class Path<TN> : IEnumerable<TN>
    {
        public TN LastStep { get; private set; }
        public Path<TN> PreviousSteps { get; private set; }
        public double TotalCost { get; private set; }

        private Path(TN lastStep, Path<TN> previousSteps, double totalCost)
        {
            LastStep = lastStep;
            PreviousSteps = previousSteps;
            TotalCost = totalCost;
        }
        public Path(TN start) : this(start, null, 0) { }
        public Path<TN> AddStep(TN step, double stepCost)
        {
            return new Path<TN>(step, this, TotalCost + stepCost);
        }

        public IEnumerator<TN> GetEnumerator()
        {
            for (Path<TN> p = this; p != null; p = p.PreviousSteps)
                yield return p.LastStep;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}