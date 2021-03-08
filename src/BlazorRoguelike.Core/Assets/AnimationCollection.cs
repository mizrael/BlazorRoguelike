using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.AspNetCore.Components;

namespace BlazorRoguelike.Core.Assets
{
    
    public class AnimationCollection : IAsset
    {
        private readonly IDictionary<string, Animation> _animations;

        public AnimationCollection(string name)
        {
            this.Name = name;

            _animations = new Dictionary<string, Animation>();
        }

        public string Name { get; }

        public IEnumerable<Animation> Animations => _animations.Values;

        public Animation GetAnimation(string name) => string.IsNullOrWhiteSpace(name) || !_animations.ContainsKey(name) ? null : _animations[name];

        private void AddAnimation(Animation animation)
        {
            if (animation == null) 
                throw new ArgumentNullException(nameof(animation));
            if(_animations.ContainsKey(animation.Name))
                throw new ArgumentException($"there is already an animation with the same name: {animation.Name}");

            _animations.Add(animation.Name, animation);
        }

        public class Animation
        {
            public Animation(string name, int fps, Size frameSize, int framesCount,
                ElementReference imageRef, string imageData, Size imageSize,
                AnimationCollection animations)
            {
                Name = name;
                Fps = fps;
                FrameSize = frameSize;
                HalfFrameSize = frameSize/2;
                FramesCount = framesCount;
                ImageRef = imageRef;
                ImageData = imageData;
                ImageSize = imageSize;
             
                animations.AddAnimation(this);
            }
            public string Name { get; }
            public int Fps { get; }
            public int FramesCount { get; }
            public Size FrameSize { get; }
            public Size HalfFrameSize { get; }
            public Size ImageSize { get; }
            public ElementReference ImageRef { get; set; }
            public string ImageData { get; }
        }
    }
}