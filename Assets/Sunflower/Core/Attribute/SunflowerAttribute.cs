using System;

namespace Sunflower.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SunflowerAttribute: Attribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class EntryAttribute: SunflowerAttribute
    {
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class AddonAttribute: SunflowerAttribute
    {
    }
}