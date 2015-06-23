using System;

namespace KinderChat.Helpers
{
    public class DisposableAction : IDisposable
    {
        private readonly Action action;

        public DisposableAction(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            if (action != null)
                action();
        }
    }
}
