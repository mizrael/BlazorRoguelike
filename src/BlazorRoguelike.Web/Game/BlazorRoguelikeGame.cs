using System.Drawing;
using System.Threading.Tasks;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.GameServices;
using BlazorRoguelike.Web.Game.GameServices;
using Blazor.Extensions;
using BlazorRoguelike.Web.Shared;
using BlazorRoguelike.Core.Web.Components;

namespace BlazorRoguelike.Web.Game
{

    public class BlazorRoguelikeGame : GameContext
    {
        private readonly IAssetsResolver _assetsResolver;
                
        public BlazorRoguelikeGame(CanvasManagerBase canvasManager,
                              IAssetsResolver assetsResolver,
                              ISoundService soundService) : base(canvasManager)
        {            
            _assetsResolver = assetsResolver;
            this.AddService(soundService);
            this.AddService(new InputService());
        }

        protected override async ValueTask Init()
        {
            var playScene = new Scenes.PlayScene(this, _assetsResolver);
            this.SceneManager.AddScene(SceneNames.Play, playScene);

            await this.SceneManager.SetCurrentScene(SceneNames.Play);
        }
    }
}