using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using System.IO;

namespace Lynxy.Security
{
    /// <summary>
    /// Utility class that handles encryption
    /// </summary>
    public static class AESEncryption
    {
        #region Static Functions

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="PlainText">Text to be encrypted</param>
        /// <param name="Password">Password to encrypt with</param>
        /// <param name="Salt">Salt to encrypt with</param>
        /// <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="PasswordIterations">Number of iterations to do</param>
        /// <param name="InitialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="KeySize">Can be 128, 192, or 256</param>
        /// <returns>An encrypted string</returns>
        public static byte[] Encrypt(byte[] Data, string Password,
            string Salt, string HashAlgorithm,
            int PasswordIterations, string InitialVector,
            int KeySize)
        {
            try
            {
                if (Data == null || Data.Length <= 0)
                    return new byte[0];
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
                PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
                byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
                RijndaelManaged SymmetricKey = new RijndaelManaged();
                SymmetricKey.Mode = CipherMode.CBC;
                byte[] CipherTextBytes = null;
                using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
                {
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                        {
                            CryptoStream.Write(Data, 0, Data.Length);
                            CryptoStream.FlushFinalBlock();
                            CipherTextBytes = MemStream.ToArray();
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }
                SymmetricKey.Clear();
                return CipherTextBytes;
            }
            catch { throw; }
        }

        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="CipherText">Text to be decrypted</param>
        /// <param name="Password">Password to decrypt with</param>
        /// <param name="Salt">Salt to decrypt with</param>
        /// <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
        /// <param name="PasswordIterations">Number of iterations to do</param>
        /// <param name="InitialVector">Needs to be 16 ASCII characters long</param>
        /// <param name="KeySize">Can be 128, 192, or 256</param>
        /// <returns>A decrypted string</returns>
        public static byte[] Decrypt(byte[] Data, string Password,
            string Salt, string HashAlgorithm,
            int PasswordIterations, string InitialVector,
            int KeySize)
        {
            try
            {
                if (Data == null || Data.Length <= 0)
                    return new byte[0];
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
                byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
                PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
                byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
                RijndaelManaged SymmetricKey = new RijndaelManaged();
                SymmetricKey.Mode = CipherMode.CBC;
                byte[] PlainTextBytes = new byte[Data.Length];
                int ByteCount = 0;
                using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
                {
                    using (MemoryStream MemStream = new MemoryStream(Data))
                    {
                        using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                        {

                            ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
                            MemStream.Close();
                            CryptoStream.Close();
                        }
                    }
                }
                SymmetricKey.Clear();
                byte[] ret = new byte[ByteCount];
                Array.Copy(PlainTextBytes, 0, ret, 0, ByteCount);
                return ret;
            }
            catch { throw; }
        }

        #endregion
    }
}
