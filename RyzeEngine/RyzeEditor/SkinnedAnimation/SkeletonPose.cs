using System.Collections.Generic;
using SharpDX;

namespace RyzeEditor.SkinnedAnimation
{
    public class SkeletonPose
    {
        public List<JointPose> JointPoses;

        public List<Matrix> GlobalPoses;//in Model space

        public Skeleton Skeleton { get; set; }
    }
}