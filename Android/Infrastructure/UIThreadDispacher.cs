using System;

namespace KinderChat.Infrastructure
{
    public class UIThreadDispacher : IUIThreadDispacher
    {
        public void Dispatch(Action action)
        {
            BaseActivity.CurrentActivity.RunOnUiThread(action);
        }
    }
}