using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BlazorRoguelike.Core.Assets.Loaders;

namespace BlazorRoguelike.Core.Assets
{
    public class AssetsResolver : IAssetsResolver
    {
        private readonly ConcurrentDictionary<string, IAsset> _assets;
        private readonly IAssetLoaderFactory _assetLoaderFactory;

        public AssetsResolver(IAssetLoaderFactory assetLoaderFactory)
        {
            _assetLoaderFactory = assetLoaderFactory;
            _assets = new ConcurrentDictionary<string, IAsset>();
        }

        public async ValueTask<TA> Load<TA>(AssetMeta meta) where TA : IAsset
        {
            if (meta == null) 
                throw new ArgumentNullException(nameof(meta));
            
            var loader = _assetLoaderFactory.Get<TA>();
            var asset = await loader.Load(meta);

            if (null == asset)
                throw new TypeLoadException($"unable to load asset type '{typeof(TA)}' from path '{meta.Path}'"); 
            
            _assets.AddOrUpdate(meta.Path, k => asset, (k, v) => asset);
            return asset;
        }

        public TA Get<TA>(string path) where TA : class, IAsset => _assets[path] as TA;
    }
}