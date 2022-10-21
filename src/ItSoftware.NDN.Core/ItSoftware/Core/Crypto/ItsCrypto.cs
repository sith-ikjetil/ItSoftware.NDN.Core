using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
namespace ItSoftware.Core.Crypto
{
	public static class ItsCrypto
	{		
		private static string EncryptionSalt = "QjO(kdLJ";

		#region Encryption Methods
		/// <summary>
		/// Encrypts a string to a byte array.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static byte[] Encrypt(string source, string key)
		{
			byte[] data = Encoding.UTF8.GetBytes(source);
			return ItsCrypto.Encrypt(data, key);

		}
		/// <summary>
		/// Encrypts a byte array to a byte array.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static byte[] Encrypt(byte[] source, string key)
		{
			Aes aes = null!;
			MemoryStream memoryStream = null!;
			CryptoStream cryptoStream = null!;

			try
			{
				//Generate a Key based on a Password and HMACSHA1 pseudo-random number generator
				//Salt must be at least 8 bytes long
				//Use an iteration count of at least 1000
				Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(ItsCrypto.EncryptionSalt), 1000);

				//Create AES algorithm
				aes = Aes.Create();
				//Key derived from byte array with 32 pseudo-random key bytes
				aes.Key = rfc2898.GetBytes(32);
				//IV derived from byte array with 16 pseudo-random key bytes
				aes.IV = rfc2898.GetBytes(16);

				//Create Memory and Crypto Streams
				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

				//Encrypt Data
				byte[] data = source;// Encoding.UTF8.GetBytes(text);
				cryptoStream.Write(data, 0, data.Length);
				cryptoStream.FlushFinalBlock();

				//Return Base 64 String
				return memoryStream.ToArray();
			}
			finally
			{
				if (cryptoStream != null)
					cryptoStream.Close();

				if (memoryStream != null)
					memoryStream.Close();

				if (aes != null)
					aes.Clear();
			}
		}
		/// <summary>
		/// Encrypts a string to a base64 encoded string.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string EncryptToBase64(string source, string key)
		{
			return Convert.ToBase64String(ItsCrypto.Encrypt(source, key));
		}
		/// <summary>
		/// Encrypts a byte array to a base 64 encoded string.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string EncryptToBase64(byte[] source, string key)
		{
			return Convert.ToBase64String(ItsCrypto.Encrypt(source, key));
		}
		#endregion

		#region Decryption Methods
		/// <summary>
		/// Decrypts a string encoded as base64 to a byte array.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static byte[] DecryptFromBase64(string source, string key)
		{
			byte[] data = Convert.FromBase64String(source);
			return ItsCrypto.Decrypt(data, key);
		}
		/// <summary>
		/// Decrypts a string encoded as base64 to a byte array converted to an utf8 string.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string DecryptFromBase64ToString(string source, string key)
		{
			byte[] data = ItsCrypto.DecryptFromBase64(source, key);
			return Encoding.UTF8.GetString(data);
		}
		/// <summary>
		/// Decrypts a byte array to a byte array.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static byte[] Decrypt(byte[] source, string key)
		{
			Aes aes = null!;
			MemoryStream memoryStream = null!;

			try
			{
				//Generate a Key based on a Password and HMACSHA1 pseudo-random number generator
				//Salt must be at least 8 bytes long
				//Use an iteration count of at least 1000
				Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(ItsCrypto.EncryptionSalt), 1000);

				//Create AES algorithm
				aes = Aes.Create();
				//Key derived from byte array with 32 pseudo-random key bytes
				aes.Key = rfc2898.GetBytes(32);
				//IV derived from byte array with 16 pseudo-random key bytes
				aes.IV = rfc2898.GetBytes(16);

				//Create Memory and Crypto Streams
				memoryStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

				//Decrypt Data
				byte[] data = source;// Convert.FromBase64String(this.textBoxEncrypt.Text);//dataToDecrypt);
				cryptoStream.Write(data, 0, data.Length);
				cryptoStream.FlushFinalBlock();

				//Return Decrypted String
				byte[] decryptBytes = memoryStream.ToArray();

				//Dispose
				if (cryptoStream != null)
					cryptoStream.Dispose();

				//Retval
				return decryptBytes;//this.textBoxResult.Text = Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
			}
			finally
			{
				if (memoryStream != null)
					memoryStream.Dispose();

				if (aes != null)
					aes.Clear();
			}
		}
		#endregion
	}
}
