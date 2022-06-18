using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TT2Master.Helpers
{
    /// <summary>
    /// Utility class for reading the savefile of tap titans 2.
    /// </summary>
    public static class SaveFileReader
    {
        /// <summary>
        /// Encryption Key as string.
        /// </summary>
        public const string SaveFileEncryptionKeyString = "4bc07927192f4e9a";

        /// <summary>
        /// Decrypts File at path using single DES in chiper mode with zero padding.
        /// </summary>
        /// <param name="path">Path to an .adat file</param>
        /// <returns>Decrypted text from the file</returns>
        public static string DecryptSaveFile(string path)
        {
            byte[] encryptedFile = File.ReadAllBytes(path);
            byte[] vectorBytes = encryptedFile.Take(8).ToArray();
            byte[] encryptedSaveBytes = encryptedFile.Skip(8).ToArray();
            byte[] decryptKeyBytes = Enumerable.Range(0, SaveFileEncryptionKeyString.Length)
                                    .Where(x => x % 2 == 0)
                                    .Select(x => Convert.ToByte(SaveFileEncryptionKeyString.Substring(x, 2), 16))
                                    .ToArray();

            string decryptedSave = DecryptMessageWithSingleDES(encryptedSaveBytes, decryptKeyBytes, vectorBytes);
            return decryptedSave;
        }

        /// <summary>
        /// Decrypts File at path using single DES in chiper mode with zero padding async.
        /// </summary>
        /// <param name="path">Path to an .adat file</param>
        /// <returns>Decrypted text from the file</returns>
        public static async Task<string> DecryptSaveFileAsync(string path)
        {
            byte[] encryptedFile = File.ReadAllBytes(path);
            byte[] vectorBytes = encryptedFile.Take(8).ToArray();
            byte[] encryptedSaveBytes = encryptedFile.Skip(8).ToArray();
            byte[] decryptKeyBytes = Enumerable.Range(0, SaveFileEncryptionKeyString.Length)
                                    .Where(x => x % 2 == 0)
                                    .Select(x => Convert.ToByte(SaveFileEncryptionKeyString.Substring(x, 2), 16))
                                    .ToArray();

            string decryptedSave = await DecryptMessageWithSingleDESAsync(encryptedSaveBytes, decryptKeyBytes, vectorBytes);
            return decryptedSave;
        }

        /// <summary>
        /// Decrypts message with given key and vector using single DES in chiper mode with zero padding.
        /// </summary>
        /// <param name="message">Message to decrypt</param>
        /// <param name="key">Encryptionkey</param>
        /// <param name="vector">Init. Vector for DES</param>
        /// <returns>Decrypted message as text</returns>
        private static string DecryptMessageWithSingleDES(byte[] message, byte[] key, byte[] vector)
        {
            string decryptedMessage = null;

            using (var desCryptoServiceProvider = new DESCryptoServiceProvider())
            {
                desCryptoServiceProvider.Key = key;
                desCryptoServiceProvider.IV = vector;
                desCryptoServiceProvider.Padding = PaddingMode.Zeros;

                var decryptor = desCryptoServiceProvider.CreateDecryptor(desCryptoServiceProvider.Key, desCryptoServiceProvider.IV);

                using (var msDecrypt = new MemoryStream(message))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            decryptedMessage = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return decryptedMessage;
        }

        /// <summary>
        /// Decrypts message with given key and vector using single DES in chiper mode with zero padding async.
        /// </summary>
        /// <param name="message">Message to decrypt</param>
        /// <param name="key">Encryptionkey</param>
        /// <param name="vector">Init. Vector for DES</param>
        /// <returns>Decrypted message as text</returns>
        private static async Task<string> DecryptMessageWithSingleDESAsync(byte[] message, byte[] key, byte[] vector)
        {
            string decryptedMessage = null;

            using (var desCryptoServiceProvider = new DESCryptoServiceProvider())
            {
                desCryptoServiceProvider.Key = key;
                desCryptoServiceProvider.IV = vector;
                desCryptoServiceProvider.Padding = PaddingMode.Zeros;

                var decryptor = desCryptoServiceProvider.CreateDecryptor(desCryptoServiceProvider.Key, desCryptoServiceProvider.IV);

                using (var msDecrypt = new MemoryStream(message))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            decryptedMessage = await srDecrypt.ReadToEndAsync();
                        }
                    }
                }
            }

            return decryptedMessage;
        }
    }
}
