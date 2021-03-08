﻿using System;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace BlazorRoguelike.Core.Assets.Loaders
{
    public class SpriteSheetAssetLoader : IAssetLoader<SpriteSheet>
    {
        private readonly HttpClient _httpClient;

        public SpriteSheetAssetLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async ValueTask<SpriteSheet> Load(AssetMeta meta)
        {
            var dto = await _httpClient.GetFromJsonAsync<SpriteSheetDTO>(meta.Path);

            var elementRef = new ElementReference(Guid.NewGuid().ToString());

            var sprites = dto.sprites
                .Select(s => new SpriteBase(s.name, elementRef, new Rectangle(s.x, s.y, s.width, s.height)))
                .ToArray();

            return new SpriteSheet(meta.Path, elementRef, dto.imagePath, sprites);
        }

        internal class SpriteSheetDTO
        {
            public string imagePath { get; set; }

            public SpriteDTO[] sprites { get; set; }

            internal class SpriteDTO
            {
                public string name { get; set; }
                public int x { get; set; }
                public int y { get; set; }
                public int width { get; set; }
                public int height { get; set; }
            }
        }
    }
}