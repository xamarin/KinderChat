using System;
using System.Threading.Tasks;

namespace KinderChat
{
    public interface ILiveConnection
    {
        event Action Closed;

        event Action<Exception> Error;

        Task ConnectAsync(string url);

        void Send(string method, object content);

        void Subscribe<TData>(string eventName, Action<TData> dataCallback);

        void Close();

        LiveConnectionState State { get; }
    }

    public enum LiveConnectionState
    {
        Closed,
        Opening,
        Opened
    }
}
