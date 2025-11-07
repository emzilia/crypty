using System.Text;
using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

class Decrypt
{
	const int MaxTagAndNonceSize = 28;

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

		if (!File.Exists(filePath)) {
			Console.Error.WriteLine("Error: Unable to load file for decryption");
			Environment.Exit(1);
		}

		byte[] ciphertext = File.ReadAllBytes(filePath);

		byte[] key = new byte[32];

		try {
			string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "key");
			key = File.ReadAllBytes(keyPath);
		} catch {
			Console.Error.WriteLine("Error: No key found - ensure the key is located in the same directory");
			Environment.Exit(1);
		}

		byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
		Array.Copy(ciphertext, 0, nonce, 0, AesGcm.NonceByteSizes.MaxSize);

		byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];
		Array.Copy(ciphertext, ciphertext.Length - AesGcm.TagByteSizes.MaxSize, tag, 0, AesGcm.TagByteSizes.MaxSize);

		byte[] cipher = new byte[ciphertext.Length - MaxTagAndNonceSize];
		Array.Copy(ciphertext, AesGcm.NonceByteSizes.MaxSize, cipher, 0, cipher.Length);

		using var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);

		byte[] plaintextBytes = new byte[ciphertext.Length - MaxTagAndNonceSize];

		aes.Decrypt(nonce, cipher, tag, plaintextBytes);

		File.WriteAllBytes(decryptPath, plaintextBytes);

		fileName = fileName.Replace (".enc", ".dec");

		ExtractArchive(fileName);
	}

	private static (byte[], byte[]) CreatePbkdf2Key(byte[] salt)
	{
		Console.WriteLine("Enter file password: ");

		string pwdString = Console.ReadLine();

		int saltSize = 16; 

		byte[] pwd = Encoding.Unicode.GetBytes(pwdString);
		
		if (salt.Length == 0)
		{
			salt = new byte[saltSize];
			using RandomNumberGenerator rng = RandomNumberGenerator.Create();
			rng.GetBytes(salt);
		}

		byte[] key = Rfc2898DeriveBytes.Pbkdf2(pwd, salt, 100000, HashAlgorithmName.SHA256, 32);

		return (key, salt);
	}


	public static void DecryptPass(string fileName)
	{
		int keySize = 32;
		int saltSize = 16;

		string decryptPath = Path.Combine(Directory.GetCurrentDirectory(), fileName.Replace(".enc", ".dec"));

		string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

		if (!File.Exists(filePath)) {
			Console.Error.WriteLine("Error: Unable to load file for decryption");
			Environment.Exit(1);
		}

		byte[] ciphertext = File.ReadAllBytes(filePath);

		byte[] salt = new byte[saltSize];
		Array.Copy(ciphertext, 0, salt, 0, saltSize);

		(byte[] key, byte[] salt) kdf = CreatePbkdf2Key(salt);

		byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
		Array.Copy(ciphertext, saltSize, nonce, 0, AesGcm.NonceByteSizes.MaxSize);

		byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];
		Array.Copy(ciphertext, ciphertext.Length - AesGcm.TagByteSizes.MaxSize, tag, 0, AesGcm.TagByteSizes.MaxSize);

		byte[] cipher = new byte[ciphertext.Length - MaxTagAndNonceSize - saltSize];
		Array.Copy(ciphertext, AesGcm.NonceByteSizes.MaxSize + saltSize, cipher, 0, cipher.Length);

		using var aes = new AesGcm(kdf.key, AesGcm.TagByteSizes.MaxSize);

		byte[] plaintextBytes = new byte[ciphertext.Length - MaxTagAndNonceSize - saltSize];

		aes.Decrypt(nonce, cipher, tag, plaintextBytes);

		File.WriteAllBytes(decryptPath, plaintextBytes);

		fileName = fileName.Replace (".enc", ".dec");

		ExtractArchive(fileName);
	}
}
