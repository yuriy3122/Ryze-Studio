using SharpDX;

namespace RyzeEditor.SkinnedAnimation
{
    /// <summary>
    /// The pose of a joint is defined as the joint’s position, 
    /// orientation and scale, relative to some frame of reference
    /// </summary>
    public class JointPose
    {
        public ushort Id { get; set; }

        public ushort ParentId { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Scale { get; set; }
    }
}