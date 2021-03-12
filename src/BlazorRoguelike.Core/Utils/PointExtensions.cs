using System.Drawing;

namespace BlazorRoguelike.Core.Utils
{
    public static class PointExtensions{
        public static int DistanceSquared(this Point a, Point b){
            var dx = (b.X-a.X);
            var dy = (b.Y-a.Y);
            return dx*dx+dy*dy;
        }
        
    }
}
