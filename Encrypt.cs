// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Security.Cryptography;

class Encrypt
{
	public static void EncryptKey(string fileName)
	{
		string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "key");
		string plaintextPath = fileName;
		string ciphertextPath = fileName + ".enc";

		var key = new byte[32];
		RandomNumberGenerator.Fill(key);
		using var aes = new AesGcm(key, 16);

		var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
		RandomNumberGenerator.Fill(nonce);

		var plaintext = File.ReadAllText(plaintextPath);
		var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

		var ciphertext = new byte[plaintextBytes.Length];

		var tag = new byte[AesGcm.TagByteSizes.MaxSize];

		File.WriteAllBytes(keyPath, key);

		aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

		File.WriteAllBytes(ciphertextPath, nonce);
		File.AppendAllBytes(ciphertextPath, ciphertext);
		File.AppendAllBytes(ciphertextPath, tag);
	}
}
