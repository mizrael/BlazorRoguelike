using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorRoguelike.Core.Components;
using BlazorRoguelike.Core.GameServices;

namespace BlazorRoguelike.Core
{
    public class GameObject 
    {
        private static int _lastId = 0;

        private readonly List<GameObject> _children = new();

        public GameObject(GameServices.Scene scene) : this(scene, ""){}

        public GameObject(GameServices.Scene scene, string name)
        {
            this.Scene = scene ?? throw new ArgumentNullException(nameof(scene));
            this.Name = name;
            this.Id = ++_lastId;

            this.Components = new ComponentsCollection(this);

            this.Scene.Register(this);
        }

        public int Id { get; }

        public ComponentsCollection Components { get; }

        public IEnumerable<GameObject> Children => _children;
        public GameObject Parent { get; private set; }

        public OnDisabledHandler OnDisabled;
        public delegate void OnDisabledHandler(GameObject gameObject);
        
        private bool _enabled = true;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                var oldValue = _enabled;
                _enabled = value;
                if(!_enabled && oldValue)
                    this.OnDisabled?.Invoke(this);
            }
        }

        public Scene Scene { get; }
        public string Name { get; }

        public OnChildAddedHandler OnChildAdded;
        public delegate void OnChildAddedHandler(GameObject sender, GameObject child);

        public OnChildRemovedHandler OnChildRemoved;
        public delegate void OnChildRemovedHandler(GameObject sender, GameObject child);

        public void AddChild(GameObject child)
        {
            if (this.Equals(child.Parent))
                return;
            
            child.Parent?._children.Remove(child);
            child.Parent = this;
            _children.Add(child);
            OnChildAdded?.Invoke(this, child);
        }

        public void RemoveChild(GameObject child){
            if (!this.Equals(child.Parent))
                return;
            child.Parent = null;
            _children.Remove(child);
            OnChildRemoved?.Invoke(this, child);
        }

        public async ValueTask Update(GameContext game)
        {
            if (!Enabled)
                return;
            
            foreach (var component in this.Components)
                await component.Update(game);

            foreach (var child in _children)
                await child.Update(game);
        }

        public override int GetHashCode() => this.Id;

        public override bool Equals(object obj) => obj is GameObject node && this.Id.Equals(node.Id);

        public override string ToString() => $"GameObject {this.Id}";
    }
}