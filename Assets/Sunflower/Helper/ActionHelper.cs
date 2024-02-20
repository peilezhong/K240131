using System;


namespace ASunflower.Helper
{
    public class ActionHelper
    {
        public static Action ActionWrapper(Action action, Action before = null, Action after = null)
        {
            return () =>
            {
                before?.Invoke();
                action?.Invoke();
                after?.Invoke();
            };
        }

        public static Action<A, B> ActionWrapper<A, B>(Action<A, B> action, Action<A, B> before = null, Action<A, B> after = null)
        {
            Action<A, B> value = (A a, B b) =>
                        {
                            before?.Invoke(a, b);
                            action?.Invoke(a, b);
                            after?.Invoke(a, b);
                        };
            return value;
        }
    }
}
