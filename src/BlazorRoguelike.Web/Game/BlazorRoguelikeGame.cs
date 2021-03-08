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
        public BlazorRoguelikeGame(BECanvasComponent canvas,
                              BECanvasComponent mapCanvas,
                              IAssetsResolver assetsResolver,
                              ISoundService soundService) : base(canvas)
        {
            this.AddService(soundService);
            this.AddService(new InputService());

            var playScene = new Scenes.PlayScene(this, assetsResolver, mapCanvas);
            this.SceneManager.AddScene(SceneNames.Play, playScene);
        }

        protected override async ValueTask Init()
        {
            

            await this.SceneManager.SetCurrentScene(SceneNames.Play);
        }
    }
}