using System.Text;
using System;
//using Windows.Storage.Streams;
//using Windows.Security.Cryptography;
//using Windows.Security.Cryptography.Core;

namespace Kitablet
{
    /// <summary>
    /// Common cryptographic helper
    /// </summary>
    public static class Crypto
    {
        /*
        /// <summary>
        /// Creates Salt with given length in bytes.
        /// </summary>
        /// <param name="lengthInBytes">No. of bytes</param>
        /// <returns></returns>
        public static byte[] CreateSalt(uint lengthInBytes)
        {
            return WinRTCrypto.CryptographicBuffer.GenerateRandom(Convert.ToInt32(lengthInBytes));
        }

        /// <summary>
        /// Creates a derived key from a comnination 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="keyLengthInBytes"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static byte[] CreateDerivedKey(string password, byte[] salt, int keyLengthInBytes = 32, int iterations = 1000)
        {
            byte[] key = new byte[] { };
            try
            {
                key = NetFxCrypto.DeriveBytes.GetBytes(password, salt, iterations, keyLengthInBytes);
                return key;
            }
            catch (Exception ex)
            {
                return key;
            }
        }
     
        /// <summary>
        /// Encrypts given data using symmetric algorithm AES
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Encrypted bytes</returns>
        public static byte[] EncryptAes(string data, string password, byte[] salt)
        {
            try
            {
                byte[] key = CreateDerivedKey(password, salt);

                ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
                ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
                var bytes = WinRTCrypto.CryptographicEngine.Encrypt(symetricKey, Encoding.UTF8.GetBytes(data));
                return bytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Decrypts given bytes using symmetric alogrithm AES
        /// </summary>
        /// <param name="data">data to decrypt</param>
        /// <param name="password">Password used for encryption</param>
        /// <param name="salt">Salt used for encryption</param>
        /// <returns></returns>
        public static string DecryptAes(byte[] data, string password, byte[] salt)
        {
            try
            {
                byte[] key = CreateDerivedKey(password, salt);

                ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
                ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
                var bytes = WinRTCrypto.CryptographicEngine.Decrypt(symetricKey, data);
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);                
            }
            catch (Exception ex)
            {
                return null;
            }            
        }
        */
        /*
        private static IBuffer GenerateKeyMaterial(string password, string salt, uint iterationCount)
        {
            // Setup KDF parameters for the desired salt and iteration count
            IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf8);
            KeyDerivationParameters kdfParameters = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, iterationCount);

            // Get a KDF provider for PBKDF2, and store the source password in a Cryptographic Key
            KeyDerivationAlgorithmProvider kdf = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha256);
            IBuffer passwordBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            CryptographicKey passwordSourceKey = kdf.CreateKey(passwordBuffer);

            // Generate key material from the source password, salt, and iteration count.  Only call DeriveKeyMaterial once,
            // since calling it twice will generate the same data for the key and IV.
            //int keySize = 256 / 8;
            //int ivSize = 128 / 8;
            //uint totalDataNeeded = (uint)(keySize + ivSize);
            //IBuffer keyAndIv = CryptographicEngine.DeriveKeyMaterial(passwordSourceKey, kdfParameters, totalDataNeeded);

            IBuffer keyAndIv = CryptographicEngine.DeriveKeyMaterial(passwordSourceKey, kdfParameters, (uint)32);
            return keyAndIv;
        }

        public static string Encrypt(string dataToEncrypt, string password, string salt)
        {
            try
            {
                // Generate a key and IV from the password and salt
                IBuffer aesKeyMaterial;
                uint iterationCount = 10000;
                aesKeyMaterial = GenerateKeyMaterial(password, salt, iterationCount);

                IBuffer plainText = CryptographicBuffer.ConvertStringToBinary(dataToEncrypt, BinaryStringEncoding.Utf8);

                // Setup an AES key, using AES in CBC mode and applying PKCS#7 padding on the input
                SymmetricKeyAlgorithmProvider aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                CryptographicKey aesKey = aesProvider.CreateSymmetricKey(aesKeyMaterial);

                // Encrypt the data and convert it to a Base64 string
                IBuffer encrypted = CryptographicEngine.Encrypt(aesKey, plainText, null);
                return CryptographicBuffer.EncodeToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Decrypt(string dataToDecrypt, string password, string salt)
        {
            try
            {
                // Generate a key and IV from the password and salt
                IBuffer aesKeyMaterial;
                uint iterationCount = 10000;
                aesKeyMaterial = GenerateKeyMaterial(password, salt, iterationCount);

                // Setup an AES key, using AES in CBC mode and applying PKCS#7 padding on the input
                SymmetricKeyAlgorithmProvider aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                CryptographicKey aesKey = aesProvider.CreateSymmetricKey(aesKeyMaterial);

                // Convert the base64 input to an IBuffer for decryption
                IBuffer ciphertext = CryptographicBuffer.DecodeFromBase64String(dataToDecrypt);

                // Decrypt the data and convert it back to a string
                IBuffer decrypted = CryptographicEngine.Decrypt(aesKey, ciphertext, null);

                byte[] decryptedArray = new byte[decrypted.Length];
                using (var reader = DataReader.FromBuffer(decrypted))
                {
                    reader.ReadBytes(decryptedArray);
                }
                //byte[] decryptedArray = decrypted.ToArray();
                return Encoding.UTF8.GetString(decryptedArray, 0, decryptedArray.Length);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        */
    }
}
