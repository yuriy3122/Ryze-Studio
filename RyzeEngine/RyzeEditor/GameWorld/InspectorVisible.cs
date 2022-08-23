using System;

namespace RyzeEditor.GameWorld
{
    public class InspectorVisible : Attribute
    {
        public bool IsVisible { get; set; }

        public InspectorVisible(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}