// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

class Encrypt
{
	public static string CreateArchive(string fileName)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			ZipFile.CreateFromDirectory(fileName, fileName + ".zip");
			fileName = fileName + ".zip";
		} else {
			TarFile.CreateFromDirectory(fileName, fileName + ".tar", false);
			fileName = fileName + ".tar";
		}
		return fileName;
	}

	public static void EncryptKey(string fileName)
	{
		string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "key");
		string plaintextPath = fileName;
		string ciphertextPath = fileName + ".enc";

		var key = new byte[32];

		if (File.Exists(keyPath)) {
			key = File.ReadAllBytes(keyPath);
		} else {
			RandomNumberGenerator.Fill(key);
		}

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
