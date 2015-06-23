using System;

namespace KinderChat
{
    public interface IUIThreadDispacher
    {
        void Dispatch(Action action);
    }
}
