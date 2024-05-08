using System.Collections.Generic;

namespace RyzeEditor.SkinnedAnimation
{
    public class AnimationClip
    {
        public Skeleton Skeleton { get; set; }

        public float FramesPerSecond { get; set; }

        public uint FrameCount { get; set; }

        public List<AnimationClip> AnimationClips { get; set; }

        public bool IsLooping { get; set; }

        public string Name { get; set; }
    }
}
