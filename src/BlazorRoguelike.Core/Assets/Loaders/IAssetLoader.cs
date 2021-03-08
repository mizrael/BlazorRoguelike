using System.Threading.Tasks;

namespace BlazorRoguelike.Core.Assets.Loaders
{
    public interface IAssetLoader<TA> where TA : IAsset
    {
        ValueTask<TA> Load(AssetMeta meta);
    }
}