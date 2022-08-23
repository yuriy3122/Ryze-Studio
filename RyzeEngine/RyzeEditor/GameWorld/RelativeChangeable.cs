using System;

namespace RyzeEditor.GameWorld
{
    public class RelativeChangeable : Attribute
    {
        public bool IsRelative { get; set; }

        public RelativeChangeable(bool isRelative)
        {
            IsRelative = isRelative;
        }
    }
}
