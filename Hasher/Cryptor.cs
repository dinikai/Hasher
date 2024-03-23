using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hasher
{
    public static class Cryptor
    {

        public static byte[] Encrypt(byte[] key, byte[] data) => Crypt(key, data, (a, b) => (byte)(a + b));
        public static byte[] Decrypt(byte[] key, byte[] data) => Crypt(key, data, (a, b) => (byte)(a - b));

        public static bool CheckChecksum(byte[] key, byte[] data, bool decrypt = true)
        {
            byte[] decrypted = decrypt ? Decrypt(key, data) : data;

            byte[] decodedChecksum = decrypted.ToList().GetRange(decrypted.Length - 32, 32).ToArray();
            decrypted = decrypted.ToList().GetRange(0, decrypted.Length - 32).ToArray();
            byte[] checksum = SHA256.HashData(decrypted);

            return decodedChecksum.SequenceEqual(checksum);
        }

        private static byte[] Crypt(byte[] key, byte[] data, Func<byte, byte, byte> func)
        {
            key = SHA256.HashData(key);
            var crypted = new List<byte>();

            int j = 0;
            for (int i = 0; i < data.Length; i++)
            {
                byte nextByte = func(data[i], key[j]);
                crypted.Add(nextByte);

                j++;
                if (j >= key.Length)
                    j = 0;
            }

            return crypted.ToArray();
        }
    }
}
