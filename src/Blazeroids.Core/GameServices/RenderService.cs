using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Core.GameServices
{
    public class RenderService : IGameService
    {
        private readonly GameContext _game;
        private readonly Canvas2DContext _context;

        public RenderService(GameContext game, Canvas2DContext context)
        {
            _game = game;
            _context = context;
        }

        public async ValueTask Step()
        {
            var layers = BuildLayers();
            
            await _context.ClearRectAsync(0, 0, _game.Display.Size.Width, _game.Display.Size.Height);

            await _context.BeginBatchAsync();
            
            foreach(var layer in layers.OrderBy(kv => kv.Key))
            foreach (var renderable in layer.Value)
                await renderable.Render(_game, _context);

            await _context.EndBatchAsync();
        }

        private IDictionary<int, IList<IRenderable>> BuildLayers()
        {
            var sceneGraph = _game.GetService<SceneGraph>();
            var layers = new Dictionary<int, IList<IRenderable>>();
            BuildLayers(sceneGraph.Root, layers);

            return layers;
        }

        private void BuildLayers(GameObject node, IDictionary<int, IList<IRenderable>> layers)
        {
            if (null == node)
                return;

            foreach (var component in node.Components)
                if (component is IRenderable renderable)
                {
                    if(!layers.ContainsKey(renderable.LayerIndex))
                        layers.Add(renderable.LayerIndex, new List<IRenderable>());
                    layers[renderable.LayerIndex].Add(renderable);
                }

            foreach (var child in node.Children)
                BuildLayers(child, layers);
        }
    }
}