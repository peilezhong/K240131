using System;
using System.ComponentModel;

namespace Sunflower.Core
{
    public class DisposeObject: IDisposable, ISupportInitialize
    {
        public virtual void Dispose()
        {
        }

        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }
    }
}