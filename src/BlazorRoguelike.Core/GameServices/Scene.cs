using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorRoguelike.Core.GameServices
{
    public abstract class Scene
    {
        private Dictionary<int, GameObject> _objectsById = new();
        private Dictionary<string, IList<GameObject>> _objectsByName = new();

        protected GameContext Game { get; }

        protected Scene(GameContext game)
        {
            this.Game = game ?? throw new ArgumentNullException(nameof(game));
        }

        public async ValueTask Step()
        {
            if (null != Root)
                await Root.Update(this.Game);
            await this.Update();
        }

        public ValueTask Enter()
        {
            this.Root = new GameObject(this);
            return this.EnterCore();
        }
        protected virtual ValueTask EnterCore() => ValueTask.CompletedTask;

        public ValueTask Exit()
        {
            this.Root = null;
            return this.ExitCore();
        }

        internal void Register(GameObject gameObject)
        {
            _objectsById[gameObject.Id] = gameObject;
            if (!_objectsByName.ContainsKey(gameObject.Name))
                _objectsByName.Add(gameObject.Name, new List<GameObject>());
            _objectsByName[gameObject.Name].Add(gameObject);
        }

        public GameObject FindGameObjectByName(string name)
        {
            return _objectsByName.ContainsKey(name) ?
                _objectsByName[name].First() : null;
        }

        protected virtual ValueTask ExitCore() => ValueTask.CompletedTask;
        protected virtual ValueTask Update() => ValueTask.CompletedTask;

        public GameObject Root { get; private set; }
    }
}