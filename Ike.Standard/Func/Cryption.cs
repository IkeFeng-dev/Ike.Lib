using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// 加解密操作
	/// </summary>
	public class Cryption
	{
		/// <summary>
		/// 异或数字,使用同一个方法进行加解密
		/// </summary>
		/// <param name="value">加密/解密值</param>
		/// <param name="key">加密/解密<see  langword="key"></see></param>
		/// <returns></returns>
		public static long XOREncryption(long value, long key)
		{
			return value ^ key;
		}

		/// <summary>
		/// 生成随机种子
		/// </summary>
		/// <returns></returns>
		public static int RandomSeed()
		{
			byte[] bytes = new byte[4];
			using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(bytes);
			}
			return BitConverter.ToInt32(bytes, 0);
		}


		/// <summary>
		/// AES加密Key支持的字节长度
		/// </summary>
		private static readonly int[] aesKeyByteLength = new int[] { 16, 24, 32 };

		/// <summary>
		/// AES加密字符串
		/// </summary>
		/// <param name="text">加密的字符串</param>
		/// <param name="key">秘钥,只能使用<see langword="16,24,32"/>字节长度的字符串作为秘钥</param>
		/// <returns>AES加密密文</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static string AesEncrypt(string text, string key)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException(nameof(text));
			}
			byte[] byteKey = Encoding.UTF8.GetBytes(key);
			if (!aesKeyByteLength.Contains(byteKey.Length))
			{
				throw new Exception("Length does not meet the requirement, the actual length is '" + byteKey.Length + "'");
			}
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = byteKey;
				aesAlg.Mode = CipherMode.ECB;
				aesAlg.Padding = PaddingMode.PKCS7;
				using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
				{
					using (var msEncrypt = new MemoryStream())
					{
						using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						{
							using (var swEncrypt = new StreamWriter(csEncrypt))
							{
								swEncrypt.Write(text);
							}
							return System.Convert.ToBase64String(msEncrypt.ToArray());
						}
					}
				}
			}
		}

		/// <summary>
		/// AES解密字符串
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="key">秘钥,只能使用<see langword="16,24,32"/>字节长度的字符串作为秘钥</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public static string AesDecrypt(string ciphertext, string key)
		{
			if (string.IsNullOrEmpty(ciphertext))
			{
				throw new ArgumentNullException(nameof(ciphertext));
			}
			byte[] byteKey = Encoding.UTF8.GetBytes(key);
			if (!aesKeyByteLength.Contains(byteKey.Length))
			{
				throw new Exception("秘钥长度不符合要求,实当前长度为 '" + byteKey.Length + "'");
			}
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = byteKey;
				aesAlg.Mode = CipherMode.ECB;
				aesAlg.Padding = PaddingMode.PKCS7;
				using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
				{
					using (var msDecrypt = new MemoryStream(System.Convert.FromBase64String(ciphertext)))
					{
						using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{
							using (var srDecrypt = new StreamReader(csDecrypt))
							{
								return srDecrypt.ReadToEnd();
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// AES加密文件
		/// </summary>
		/// <param name="sourceFile">待加密的文件的完整路径</param>
		/// <param name="outputFile">加密后的文件的完整路径</param>
		/// <param name="secretKey">秘钥,只能使用<see langword="16,24,32"/>字节长度的字符串作为秘钥</param>
		public static bool AesFileEncrypt(string sourceFile, string outputFile, string secretKey)
		{
			byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
			if (!aesKeyByteLength.Contains(keyByte.Length))
			{
				throw new ArgumentException($"秘钥字节长度不正确,正确长度为[16,24,32],当前长度为 '{keyByte.Length}'");
			}
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = keyByte;
				aesAlg.GenerateIV();
				using (FileStream fsInput = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
				{
					using (FileStream fsEncrypted = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
					{
						fsEncrypted.Write(aesAlg.IV, 0, aesAlg.IV.Length);
						using (ICryptoTransform aesEncryptor = aesAlg.CreateEncryptor())
						{
							using (CryptoStream cryptoStream = new CryptoStream(fsEncrypted, aesEncryptor, CryptoStreamMode.Write))
							{
								byte[] bytearrayinput = new byte[fsInput.Length];
								fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
								cryptoStream.Write(bytearrayinput, 0, bytearrayinput.Length);
								return File.Exists(outputFile);
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// AES文件解密
		/// </summary>
		/// <param name="sourceFile">加密文件路径</param>
		/// <param name="outputFile">解密后储存路径</param>
		/// <param name="secretKey">秘钥,只能使用<see langword="16,24,32"/>字节长度的字符串作为秘钥</param>
		/// <returns></returns>
		public static bool AesFileDecrypt(string sourceFile, string outputFile, string secretKey)
		{
			byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
			if (!aesKeyByteLength.Contains(keyByte.Length))
			{
				throw new ArgumentException($"秘钥字节长度不正确,正确长度为[16,24,32],当前长度为 '{keyByte.Length}'");
			}
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = keyByte;
				using (FileStream fsInput = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
				{
					byte[] iv = new byte[aesAlg.IV.Length];
					fsInput.Read(iv, 0, iv.Length);
					using (FileStream fsDecrypted = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
					{
						using (ICryptoTransform aesDecryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
						{
							using (CryptoStream cryptoStream = new CryptoStream(fsInput, aesDecryptor, CryptoStreamMode.Read))
							{
								cryptoStream.CopyTo(fsDecrypted);
								return File.Exists(outputFile);
							}
						}
					}
				}
			}
		}



	}
}
