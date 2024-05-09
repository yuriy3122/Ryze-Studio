using SharpDX;

namespace RyzeEditor.SkinnedAnimation
{
    /// <summary>
    /// A skeleton is comprised of a hierarchy of rigid pieces - joints
    /// Represents the current state (interpolation of the skeleton's joint poses)
    /// We except to send Joint's data to the Rendering Pipeline
    /// </summary>
    public struct Joint
    {
        public ushort ParentId { get; set; }

        public Matrix InvBindPose { get; set; }
    }
}
