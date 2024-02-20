using System;
using System.Threading;

namespace Sunflower.Core
{
    public class IDObject
    {
        public Guid Id { get; set; } = InitID(); // 在属性初始化时执行的方法  

        private static Guid InitID()
        {
            
            return Guid.NewGuid();
        }
    }
}
