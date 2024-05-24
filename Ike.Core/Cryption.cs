using System.Security.Cryptography;
using System.Text;

namespace Ike.Core
{
	/// <summary>
	/// 加密解密类
	/// </summary>
	public class Cryption
	{
		/// <summary>
		/// RSA秘钥
		/// </summary>
		public class RSASecretKey
		{
			private readonly string publicKey;
			private readonly string privateKey;
			/// <summary>
			/// 生成一对RSA秘钥
			/// </summary>
			public RSASecretKey()
			{
				using RSA rsa = RSA.Create();
				publicKey = System.Convert.ToBase64String(rsa.ExportRSAPublicKey());
				privateKey = System.Convert.ToBase64String(rsa.ExportRSAPrivateKey());
			}
			/// <summary>
			/// RSA公钥,用于加密
			/// </summary>
			public string PublicKey { get { return publicKey; } }
			/// <summary>
			/// RSA私钥,用于解密
			/// </summary>
			public string PrivateKey { get { return privateKey; } }
		}

		/// <summary>
		/// RSA加密字符串,可使用<see cref="RSASecretKey"/>生成秘钥
		/// </summary>
		/// <param name="text">需要加密的字符串</param>
		/// <param name="publicKey"><see cref="RSASecretKey"/>生成的公钥,需要使用对应的私钥进行解密,记得保留好私钥</param>
		/// <returns>密文</returns>
		public static string RsaEncryption(string text, string publicKey)
		{
			using RSA rsa = RSA.Create();
			rsa.ImportRSAPublicKey(System.Convert.FromBase64String(publicKey), out _);
			byte[] plainData = Encoding.UTF8.GetBytes(text);
			byte[] encryptedData = rsa.Encrypt(plainData, RSAEncryptionPadding.OaepSHA256);
			return System.Convert.ToBase64String(encryptedData);
		}

		/// <summary>
		/// RSA解密字符串
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="privateKey">密文的私钥</param>
		/// <returns></returns>
		public static string RsaDecryption(string ciphertext, string privateKey)
		{
			using RSA rsa = RSA.Create();
			rsa.ImportRSAPrivateKey(System.Convert.FromBase64String(privateKey), out _);
			byte[] encryptedData = System.Convert.FromBase64String(ciphertext);
			byte[] decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
			return Encoding.UTF8.GetString(decryptedData);
		}
	}
}
