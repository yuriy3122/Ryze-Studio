using SharpDX;

namespace RyzeEditor.SkinnedAnimation
{
    /// <summary>
    /// A skeleton is comprised of a hierarchy of rigid pieces - boness
    /// Represents the current state (interpolation of the skeleton's bone poses)
    /// We except to send Bone's data to the Rendering Pipeline
    /// </summary>
    public struct Bone
    {
        public ushort ParentId { get; set; }

        public Matrix InvBindPose { get; set; }
    }
}
