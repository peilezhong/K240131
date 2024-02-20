using System;
using Sunflower.Core;

namespace Sunflower.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIAttribute: SunflowerAttribute
    {
        public string UiFile;
        public UILayer UiLayer;
        
        public UIAttribute(string uiFile, UILayer uiLayer)
        {
            this.UiFile = uiFile;
            this.UiLayer = uiLayer;
        }
    }
}