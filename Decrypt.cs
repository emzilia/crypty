using System.Text;
using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

class Decrypt
{
	public static void ExtractArchive(string fileName)
	{
		if (fileName.Contains(".zip") || fileName.Contains(".tar")) {
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				ZipFile.ExtractToDirectory(fileName, fileName.Replace(".zip", ""), true);
			} else {
				string newDir = fileName.Replace(".tar", "");
				Directory.CreateDirectory(newDir);
				TarFile.ExtractToDirectory(fileName, Path.Combine(Directory.GetCurrentDirectory(), newDir), true);
			}
			if (File.Exists(fileName)) File.Delete(fileName);
		}
	}

	public static void DecryptKey(string fileName)
	{
		string decryptPath = Path.Combine(Directory.GetCurrentDirectory(), fileName.Replace(".enc", ".dec"));

		string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
		byte[] ciphertext = File.ReadAllBytes(filePath);

		byte[] key = new byte[32];

		try {
			string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "key");
			key = File.ReadAllBytes(keyPath);
		} catch {
			Console.Error.WriteLine("Error: No key found - ensure the key is located in the same directory");
			Environment.Exit(1);
		}

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

			File.WriteAllBytes(decryptPath, plaintextBytes);
		}

		fileName = fileName.Replace (".enc", ".dec");

		ExtractArchive(fileName);
	}
}
