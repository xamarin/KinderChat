using System;
using CoreFoundation;

namespace KinderChat.iOS
{
    public class UiThreadDispacher : IUIThreadDispacher
    {
        public void Dispatch(Action action)
        {
            DispatchQueue.MainQueue.DispatchAsync(action);
        }
    }
}