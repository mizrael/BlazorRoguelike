using System.Threading.Tasks;
using BlazorRoguelike.Core;
using BlazorRoguelike.Core.Assets;
using BlazorRoguelike.Core.GameServices;
using BlazorRoguelike.Web.Game.GameServices;
using BlazorRoguelike.Core.Web.Components;
using System.Drawing;

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
            var collisionService = new CollisionService(this, new Size(64, 64));
            this.AddService(collisionService);

            var playScene = new Scenes.PlayScene(this, _assetsResolver, collisionService);
            this.SceneManager.AddScene(SceneNames.Play, playScene);

            await this.SceneManager.SetCurrentScene(SceneNames.Play);
        }
    }
}