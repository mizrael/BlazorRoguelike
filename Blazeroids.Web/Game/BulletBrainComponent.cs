﻿using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazor.Extensions;

namespace Blazeroids.Web.Game
{
    public class BulletBrainComponent : BaseComponent
    {
        private readonly MovingBodyComponent _movingBody;
        private readonly TransformComponent _transformComponent;
        
        public BulletBrainComponent(GameObject owner) : base(owner)
        {
            _movingBody = owner.Components.Get<MovingBodyComponent>();
            _transformComponent = owner.Components.Get<TransformComponent>();
        }
        
        public override async ValueTask Update(GameContext game)
        {
            _movingBody.Thrust = this.Speed;

            var isOutScreen = _transformComponent.World.Position.X < 0 ||
                              _transformComponent.World.Position.Y < 0 ||
                              _transformComponent.World.Position.X > this.Canvas.Width ||
                              _transformComponent.World.Position.Y > this.Canvas.Height;
            if (isOutScreen)
                this.Owner.Enabled = false;
        }

        public float Speed { get; set; }
        public BECanvasComponent Canvas { get; set; }
    }
}