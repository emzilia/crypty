using System.Text;
using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

class Encrypt
{
	const int keySize = 32;
	const int saltSize = 16; 

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
		byte[] key = new byte[keySize];

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
		string pwd = "";
		ConsoleKeyInfo enteredKey;

		Console.Write("Enter file password: ");
		while ((enteredKey = Console.ReadKey(true)).Key != ConsoleKey.Enter) {
			if (enteredKey.Key == ConsoleKey.Backspace && pwd.Length > 0) {
				pwd = pwd.Remove(pwd.Length - 1);
				Console.Write("\b \b");
			} else if (enteredKey.Key != ConsoleKey.Backspace) {
				pwd += enteredKey.KeyChar;
				Console.Write("*");
			}
		}
		Console.WriteLine();

		byte[] pwdBytes = Encoding.Unicode.GetBytes(pwd);

		pwd = pwd.Remove(0);

		byte[] salt = new byte[saltSize];
		using RandomNumberGenerator rng = RandomNumberGenerator.Create();
		rng.GetBytes(salt);

		byte[] key = Rfc2898DeriveBytes.Pbkdf2(pwdBytes, salt, 100000, HashAlgorithmName.SHA3_256, keySize);

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
