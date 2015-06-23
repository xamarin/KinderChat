using System.Collections.Generic;

namespace KinderChat
{
    public interface ICryptoService
    {
        void GenerateKeys();

        byte[] PublicKey { get; }

        byte[] CalculateHash(byte[] content);

        string Encrypt(string text, Dictionary<string, string> publicKeyIdAndPublicKeyMap, out Dictionary<string, string> publicKeyIdAndEncryptedKeyMap);

        string Encrypt(string text, Dictionary<string, string> publicKeyIdAndPublicKeyMap, out Dictionary<string, byte[]> publicKeyIdAndEncryptedKeyMap);

        byte[] Encrypt(byte[] text, Dictionary<string, byte[]> publicKeyIdAndPublicKeyMap, out Dictionary<string, byte[]> publicKeyIdAndEncryptedKeyMap);
        
        string Decrypt(string text, string key);

        string Decrypt(string text, byte[] key);

        byte[] Decrypt(byte[] text, byte[] key);
    }
}