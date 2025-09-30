// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Security.Cryptography;

class Decrypt
{
	public static void DecryptKey(string fileName)
	{
		string decryptPath = Path.Combine(Directory.GetCurrentDirectory(), fileName.Replace(".enc", ""));

		string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
		byte[] ciphertext = File.ReadAllBytes(filePath);

		string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "key");
		byte[] key = File.ReadAllBytes(keyPath);

		byte[] nonce = new byte[12];
		Array.Copy(ciphertext, 0, nonce, 0, 12);

		byte[] tag = new byte[16];
		Array.Copy(ciphertext, ciphertext.Length - 16, tag, 0, 16);

		byte[] cipher = new byte[ciphertext.Length - 28];
		Array.Copy(ciphertext, 12, cipher, 0, cipher.Length);

		using (var aes = new AesGcm(key, 16))
		{
			byte[] plaintextBytes = new byte[ciphertext.Length - 28];

			aes.Decrypt(nonce, cipher, tag, plaintextBytes);

			string plaintext = Encoding.UTF8.GetString(plaintextBytes);

			File.WriteAllText(decryptPath, plaintext);
		}
	}
}
