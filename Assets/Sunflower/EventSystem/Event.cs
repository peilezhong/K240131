
using Sunflower.Core;

namespace Sunflower.EventSys
{

    public enum EventType
    {
        StringEvent,
        SimpleEvent
    }

    public interface IEvent
    {

    }
    public class SimpleEvent<T> : IDObject, IEvent
    {

    }
}
