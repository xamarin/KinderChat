using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCLCrypto;

namespace KinderChat
{
	public class RsaAndAesHybridCryptoService : ICryptoService
    {
        private ICryptographicKey decryptionKey;

        private byte[] publicKey;
        private byte[] privateKey;
        
        private readonly byte[] aesKeyForStoringRsaPrivateKey = {42, 123, 0, 3, 16, 88, 222, 190, 0, 1, 5, 18, 53, 83, 131, 1}; //just a random password-key for storing PrivateKey on the device (just an additional security)
        private const int RsaKeySize = 2048;
        private const int RsaEncryptionSize = RsaKeySize / 8;
        private const int AesKeySize = 32; //256 bit (AES256)
        private const int AesIvSize = AesKeySize / 2; //128 bit (AES256)

        private const AsymmetricAlgorithm RsaType = AsymmetricAlgorithm.RsaPkcs1; //or OeapSha1 and OaepSha256

        public void GenerateKeys()
        {
            ICryptographicKey key = GetAsymmetricKeyAlgorithm().CreateKeyPair(RsaKeySize);
            PrivateKey = key.Export(CryptographicPrivateKeyBlobType.Pkcs1RsaPrivateKey);
            PublicKey = key.ExportPublicKey(CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey);
        }

        public byte[] PublicKey
        {
            get
            {
				if (publicKey != null && publicKey.Length > 0)
                    return publicKey;

				publicKey = Settings.PublicKey;
				if (publicKey == null || publicKey.Length < 1)
					publicKey = RestorePublicKey ();
				
                return publicKey;
            }
            set
            {
                Settings.PublicKey = value;
                publicKey = value;
            }
        }


        public byte[] CalculateHash(byte[] content)
        {
            return WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5).HashData(content);
        }

        public string Encrypt(string text, Dictionary<string, string> publicKeyIdAndPublicKeyMap, out Dictionary<string, string> publicKeyIdAndEncryptedKeyMap)
        {
            Dictionary<string, byte[]> publicKeyIdAndEncryptedKeyMapByted;
            var result = Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(text), 
                publicKeyIdAndPublicKeyMap.ToDictionary(i => i.Key, i => Convert.FromBase64String(i.Value)),
                out publicKeyIdAndEncryptedKeyMapByted));
            publicKeyIdAndEncryptedKeyMap = publicKeyIdAndEncryptedKeyMapByted.ToDictionary(i => i.Key, i => Convert.ToBase64String(i.Value));
            return result;
        }

	    public string Encrypt(string text, Dictionary<string, string> publicKeyIdAndPublicKeyMap, out Dictionary<string, byte[]> publicKeyIdAndEncryptedKeyMap)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(text),
                publicKeyIdAndPublicKeyMap.ToDictionary(i => i.Key, i => Convert.FromBase64String(i.Value)),
                out publicKeyIdAndEncryptedKeyMap));
	    }

	    public byte[] Encrypt(byte[] data, Dictionary<string, byte[]> publicKeyIdAndPublicKeyMap, out Dictionary<string, byte[]> publicKeyIdAndEncryptedKeyMap)
        {
            publicKeyIdAndEncryptedKeyMap = new Dictionary<string, byte[]>();
            if (publicKeyIdAndPublicKeyMap == null || publicKeyIdAndPublicKeyMap.Count < 1)
                return data;

            IAsymmetricKeyAlgorithmProvider provider = GetAsymmetricKeyAlgorithm();
            byte[] randomAesKey = WinRTCrypto.CryptographicBuffer.GenerateRandom(AesKeySize);
            byte[] iv = WinRTCrypto.CryptographicBuffer.GenerateRandom(AesIvSize); //not sure it makes sence to encrypt iv as well.
            byte[] encryptedData = Aes.Encrypt(randomAesKey, data, iv);

            foreach (var item in publicKeyIdAndPublicKeyMap)
            {
                var publicKeyId = item.Key;
                var publicKey = item.Value;

                try
                {
                    ICryptographicKey encryptionKey = provider.ImportPublicKey(publicKey, CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey);
                    byte[] messageKey = WinRTCrypto.CryptographicEngine.Encrypt(encryptionKey, randomAesKey);

                    var keyBytes = new byte[messageKey.Length + iv.Length];
                    Buffer.BlockCopy(messageKey, 0, keyBytes, 0, messageKey.Length);
                    Buffer.BlockCopy(iv, 0, keyBytes, messageKey.Length, iv.Length);

                    publicKeyIdAndEncryptedKeyMap[publicKeyId] = keyBytes;
                }
                catch (Exception)
                {
                    publicKeyIdAndEncryptedKeyMap[publicKeyId] = new byte[] {0};
                }

            }

            return encryptedData;
        }

        public string Decrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(key))
                return text;

            var bytes = Decrypt(Convert.FromBase64String(text), Convert.FromBase64String(key));
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public string Decrypt(string text, byte[] key)
        {
            var bytes = Decrypt(Convert.FromBase64String(text), key);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public byte[] Decrypt(byte[] encryptedData, byte[] key)
        {
            if (key == null || key.Length < 1)
                return encryptedData;

            if (key.Length == 1)
                throw new InvalidOperationException("Key is invalid");

            byte[] messageKey = new byte[RsaEncryptionSize];
            byte[] iv = new byte[AesIvSize];

            Buffer.BlockCopy(key, 0, messageKey, 0, messageKey.Length);
            Buffer.BlockCopy(key, messageKey.Length, iv, 0, iv.Length);

            var decryptedAesKey = WinRTCrypto.CryptographicEngine.Decrypt(DecryptionKey, messageKey);
            return Aes.Decrypt(decryptedAesKey, encryptedData, iv);
        }

        private byte[] RestorePublicKey()
        {
            //we've lost public key, restore it using Private Key
			var key = GetAsymmetricKeyAlgorithm().ImportKeyPair(PrivateKey, CryptographicPrivateKeyBlobType.Pkcs1RsaPrivateKey)
                .ExportPublicKey(CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey);

			PublicKey = key;
			return key;
        }

        private byte[] PrivateKey
        {
            get
            {
				if (privateKey != null && privateKey.Length > 0)
                    return privateKey;
				privateKey = Aes.Decrypt(aesKeyForStoringRsaPrivateKey, Settings.PrivateKey);
                if (privateKey == null || privateKey.Length < 1)
                    throw new InvalidOperationException("PrivateKey is lost or is not generated yet.");
                return privateKey;
            }
            set
            {
                Settings.PrivateKey = Aes.Encrypt(aesKeyForStoringRsaPrivateKey, value);
                privateKey = value;
            }
        }

        private ICryptographicKey DecryptionKey
        {
            get
            {
                if (decryptionKey != null)
                    return decryptionKey;
                var provider = GetAsymmetricKeyAlgorithm();
                decryptionKey = provider.ImportKeyPair(PrivateKey, CryptographicPrivateKeyBlobType.Pkcs1RsaPrivateKey);
                return decryptionKey;
            }
        }

        private static IAsymmetricKeyAlgorithmProvider GetAsymmetricKeyAlgorithm()
        {
            return WinRTCrypto.AsymmetricKeyAlgorithmProvider.OpenAlgorithm(RsaType);
        }
    }
}