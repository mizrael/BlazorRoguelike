using System.Drawing;
using System.Threading.Tasks;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.GameServices;
using BlazorRoguelike.Web.Game.GameServices;
using Blazor.Extensions;

namespace BlazorRoguelike.Web.Game
{

    public class BlazorRoguelikeGame : GameContext
    {
        private readonly IAssetsResolver _assetsResolver;

        public BlazorRoguelikeGame(BECanvasComponent canvas, 
                              IAssetsResolver assetsResolver,
                              ISoundService soundService) : base(canvas)
        {
            _assetsResolver = assetsResolver;

            this.AddService(soundService);
        }

        protected override async ValueTask Init()
        {
            this.AddService(new InputService());

            this.SceneManager.AddScene(SceneNames.Play, new Scenes.PlayScene(this, _assetsResolver));

            await this.SceneManager.SetCurrentScene(SceneNames.Play);
        }
    }
}