using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.Assets.Loaders;
using BlazorRoguelike.Web.Game.Assets;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorRoguelike.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton<IAssetsResolver, AssetsResolver>();
            builder.Services.AddSingleton<IAssetLoader<Sprite>, SpriteAssetLoader>();
            builder.Services.AddSingleton<IAssetLoader<AnimationCollection>, AnimationsAssetLoader>();
            builder.Services.AddSingleton<IAssetLoader<SpriteSheet>, SpriteSheetAssetLoader>();
            builder.Services.AddSingleton<IAssetLoader<Sound>, SoundAssetLoader>();
            builder.Services.AddSingleton<IAssetLoader<MapObjects>, MapObjectsLoader>();

            builder.Services.AddSingleton<IAssetLoaderFactory>(ctx =>
            {
                var factory = new AssetLoaderFactory();
                
                factory.Register(ctx.GetRequiredService<IAssetLoader<Sprite>>());
                factory.Register(ctx.GetRequiredService<IAssetLoader<SpriteSheet>>());
                factory.Register(ctx.GetRequiredService<IAssetLoader<AnimationCollection>>());
                factory.Register(ctx.GetRequiredService<IAssetLoader<Sound>>());
                factory.Register(ctx.GetRequiredService<IAssetLoader<MapObjects>>());

                return factory;
            });

            await builder.Build().RunAsync();
        }
    }
}
