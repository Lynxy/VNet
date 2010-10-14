using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security;
using System.Security.Cryptography;

namespace Lynxy.Security
{
    public class CryptographicScheme
    {
        protected RSACryptoServiceProvider RSA_Local;
        protected RSACryptoServiceProvider RSA_Foreign;
        protected MD5CryptoServiceProvider HashMD5;
        protected SHA256 HashSHA;
        private SecureString CombinedKey = new SecureString();

        protected bool KeysCombined = false;

        public CryptographicScheme(int keySize)
        {
            RSA_Local = new RSACryptoServiceProvider(keySize);
            RSA_Foreign = new RSACryptoServiceProvider(keySize);
            HashMD5 = new MD5CryptoServiceProvider();
            HashSHA = SHA256.Create();
            
        }
        public CryptographicScheme() : this(256) { }


        public byte[] PublicKey { get { return RSA_Local.ExportCspBlob(false); } }
        public byte[] LocalEncryptionKey { get; set; }
        public byte[] ForeignEncryptionKey { get; set; }


        public void ImportForeignPublicKey(byte[] publicKey)
        {
            RSA_Foreign.ImportCspBlob(publicKey);
        }

        public byte[] Encrypt(byte[] data)
        {
            if (KeysCombined)
                return AESEncryption.Encrypt(data, CombinedKey.ToString(), "Lynxy", "SHA1", 2, "1234567890123456", 256);
            else
                return RSA_Foreign.Encrypt(data, true);
        }

        public byte[] Decrypt(byte[] data)
        {
            if (KeysCombined)
                return AESEncryption.Decrypt(data, CombinedKey.ToString(), "Lynxy", "SHA1", 2, "1234567890123456", 256);
            else
                return RSA_Local.Decrypt(data, true);
        }

        public void CombineKeys(bool localFirst)
        {
            if (LocalEncryptionKey == null || ForeignEncryptionKey == null)
                throw new Exception("Not all encryption keys were set");
            
            byte[] combinedEncryptionKey = new byte[LocalEncryptionKey.Length + ForeignEncryptionKey.Length];
            if (localFirst)
            {
                LocalEncryptionKey.CopyTo(combinedEncryptionKey, 0);
                ForeignEncryptionKey.CopyTo(combinedEncryptionKey, LocalEncryptionKey.Length);
            }
            else
            {
                ForeignEncryptionKey.CopyTo(combinedEncryptionKey, 0);
                LocalEncryptionKey.CopyTo(combinedEncryptionKey, ForeignEncryptionKey.Length);
            }

            byte[] keyArray = HashSHA.ComputeHash(combinedEncryptionKey);
            combinedEncryptionKey = null;
            HashSHA.Clear();
            for (int i = 0; i < keyArray.Length; i++)
                CombinedKey.AppendChar((char)keyArray[i]);
            //AES.Key = keyArray;
            KeysCombined = true;
        }
    }
}
