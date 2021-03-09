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
        private readonly BECanvasComponent _mapCanvas;
        private readonly IAssetsResolver _assetsResolver;

        public BlazorRoguelikeGame(BECanvasComponent canvas,
                              BECanvasComponent mapCanvas,
                              IAssetsResolver assetsResolver,
                              ISoundService soundService) : base(canvas)
        {
            _mapCanvas = mapCanvas;
            _assetsResolver = assetsResolver;
            this.AddService(soundService);
            this.AddService(new InputService());
        }

        protected override async ValueTask Init()
        {
            var playScene = new Scenes.PlayScene(this, _assetsResolver, _mapCanvas);
            this.SceneManager.AddScene(SceneNames.Play, playScene);

            await this.SceneManager.SetCurrentScene(SceneNames.Play);
        }
    }
}