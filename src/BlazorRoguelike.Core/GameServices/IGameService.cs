using System.Threading.Tasks;

namespace BlazorRoguelike.Core.GameServices
{
    public interface IGameService
    {
        ValueTask Step();
    }
}