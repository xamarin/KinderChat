using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;


namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.InMemory
{
    public class InMemoryDeviceRepository : IDevicesRepository
    {
        private readonly ConcurrentDictionary<long, List<string>> _devicesByUsers = new ConcurrentDictionary<long, List<string>>();
        private readonly ConcurrentDictionary<string, byte[]> _publicKeys = new ConcurrentDictionary<string, byte[]>();

        public List<string> GetDevices(long userId)
        {
            //return new List<string> { userId.ToString() }; //TEMP

            List<string> list;
            _devicesByUsers.TryGetValue(userId, out list);
            return list == null ? new List<string>() : list.Distinct().ToList(); 

            //if (userId == 28) return new List<string> { "8c4a53db-aa08-432c-8630-dca09c220bfc" };
            //if (userId == 29) return new List<string> { "128f9b4b-9316-4d89-afae-df365ce6a528", "b3681fa8-95f9-4b29-a926-6cdf8b946da8" };
            //return new List<string> { userId.ToString() };
        }

        public void AddDevice(long userId, string deviceId)
        {
            _devicesByUsers.ChangeListFromDictionary(userId, list => list.Add(deviceId));
        }

        public void SetPublicKeyForDevice(string deviceId, byte[] publicKey)
        {
            _publicKeys[deviceId] = publicKey;
        }

        public byte[] GetPublicKeyForDevice(string device)
        {
            return _publicKeys[device];
        }
    }
}
