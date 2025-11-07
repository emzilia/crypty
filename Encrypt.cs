using System.Text;
using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

class Encrypt
{
	private static string CreateArchiveIfDir(string fileName)
	{
		if (File.Exists(fileName)) {
			return fileName;
		} else if (Directory.Exists(fileName)) {
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
					ZipFile.CreateFromDirectory(fileName, fileName + ".zip");
					fileName = fileName + ".zip";
				} else {
					TarFile.CreateFromDirectory(fileName, fileName + ".tar", false);
					fileName = fileName + ".tar";
				}
				return fileName;
		} else {
			Console.Error.WriteLine("Error: Invalid file / Directory");
			Environment.Exit(1);
			return fileName;
		}
	}

	public static void EncryptKey(string fileName)
	{
		string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "key");
		string plaintextPath = CreateArchiveIfDir(fileName);
		string ciphertextPath = plaintextPath + ".enc";
		byte[] key = new byte[32];

		if (File.Exists(keyPath)) {
			key = File.ReadAllBytes(keyPath);
		} else {
			RandomNumberGenerator.Fill(key);
		}

		using var aes = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);

		byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
		RandomNumberGenerator.Fill(nonce);

		byte[] plaintextBytes = File.ReadAllBytes(plaintextPath);

		byte[] ciphertext = new byte[plaintextBytes.Length];

		byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

		File.WriteAllBytes(keyPath, key);

		aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

		File.WriteAllBytes(ciphertextPath, nonce);
		File.AppendAllBytes(ciphertextPath, ciphertext);
		File.AppendAllBytes(ciphertextPath, tag);

		if (File.Exists(ciphertextPath) == true) {
			if (ciphertextPath.Contains(".zip") || ciphertextPath.Contains (".tar")) {
				if (File.Exists(plaintextPath)) File.Delete(plaintextPath);
			}
		}

		Environment.Exit(0);
	}

	private static (byte[], byte[]) CreatePbkdf2Key()
	{
		Console.WriteLine("Enter file password: ");

		string pwdString = Console.ReadLine();

		int saltSize = 16; 

		byte[] pwd = Encoding.Unicode.GetBytes(pwdString);
		
		byte[] salt = new byte[saltSize];
		using RandomNumberGenerator rng = RandomNumberGenerator.Create();
		rng.GetBytes(salt);

		byte[] key = Rfc2898DeriveBytes.Pbkdf2(pwd, salt, 100000, HashAlgorithmName.SHA256, 32);

		return (key, salt);
	}

	public static void EncryptPass(string fileName)
	{
		string plaintextPath = CreateArchiveIfDir(fileName);
		string ciphertextPath = plaintextPath + ".enc";

		(byte[] key, byte[] salt) kdf = CreatePbkdf2Key();

		using var aes = new AesGcm(kdf.key, AesGcm.TagByteSizes.MaxSize);

		byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
		RandomNumberGenerator.Fill(nonce);

		byte[] plaintextBytes = File.ReadAllBytes(plaintextPath);

		byte[] ciphertext = new byte[plaintextBytes.Length];

		byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

		aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

		File.WriteAllBytes(ciphertextPath, kdf.salt);
		File.AppendAllBytes(ciphertextPath, nonce);
		File.AppendAllBytes(ciphertextPath, ciphertext);
		File.AppendAllBytes(ciphertextPath, tag);

		if (File.Exists(ciphertextPath) == true) {
			if (ciphertextPath.Contains(".zip") || ciphertextPath.Contains (".tar")) {
				if (File.Exists(plaintextPath)) File.Delete(plaintextPath);
			}
		}

		Environment.Exit(0);
	}
	
}
