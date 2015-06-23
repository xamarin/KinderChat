using System.Collections.Generic;

namespace KinderChat.WorkerRole.SocketServer.Domain
{
    public interface IDevicesRepository
    {
        List<string> GetDevices(long userId);

        void AddDevice(long userId, string deviceId);

        void SetPublicKeyForDevice(string deviceId, byte[] publicKey);

        byte[] GetPublicKeyForDevice(string device);
    }
}
