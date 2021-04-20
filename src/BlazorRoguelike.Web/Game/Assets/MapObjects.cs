using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Assets.Loaders;
using BlazorRoguelike.Web.Game.Mechanics;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorRoguelike.Web.Game.Assets
{
    public record MapObjects(string Name, IEnumerable<MapObject> Objects) : IAsset;

    public class MapObjectsLoader : IAssetLoader<MapObjects>
    {
        private readonly HttpClient _httpClient;

        public MapObjectsLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async ValueTask<MapObjects> Load(AssetMeta meta)
        {
            var dtos = await _httpClient.GetFromJsonAsync<MapObjectDTO[]>(meta.Path);
            var mapObjects = dtos.Select(d => new MapObject(d.id));

            return new MapObjects(meta.Path, mapObjects);
        }

        internal record MapObjectDTO(string id);
    }
}
