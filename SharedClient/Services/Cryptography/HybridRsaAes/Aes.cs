using PCLCrypto;

namespace KinderChat
{
    public static class Aes
    {
        private static readonly ISymmetricKeyAlgorithmProvider Provider = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);

        public static byte[] Encrypt(byte[] key, byte[] data, byte[] iv = null)
        {
            //var hash = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256).HashData(key);
            var symmericKey = Provider.CreateSymmetricKey(key);
            return WinRTCrypto.CryptographicEngine.Encrypt(symmericKey, data, iv);
        }

        public static byte[] Decrypt(byte[] key, byte[] data, byte[] iv = null)
        {
            //var hash = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256).HashData(key);
            var symmericKey = Provider.CreateSymmetricKey(key);
            return WinRTCrypto.CryptographicEngine.Decrypt(symmericKey, data, iv);
        }
    }
}
