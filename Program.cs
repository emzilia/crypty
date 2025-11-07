// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;

string help = 
@"Usage: crypty [operand] [filename]
Encrypts files with AES-GCM encryption using a keyfile.
Directories are archived with zip or tar before encryption.
  Operands:
    -e, --encrypt   encrypts filename to filename.enc
    -d, --decrypt   decrypts filename to filename.dec
    -h, --hash      provides the SHA3-256 hash of a file";

void CheckSupport(string operand)
{
	if (!AesGcm.IsSupported) {
		Console.Error.WriteLine("Error: AES-GCM isn't supported on this platform");
		Environment.Exit(1);
	}

	if (operand == "-h" || operand == "--hash") {
		if (!SHA3_256.IsSupported) {
			Console.Error.WriteLine("Error: SHA3-256 isn't supported on this platform");
			Environment.Exit(1);
		}
	}
}

if (args.Length != 2) {
	Console.WriteLine(help);
	Environment.Exit(1);
}

string operand = args[0];
string fileName = args[1];

CheckSupport(operand);

try {
	switch(operand)
	{
		case "-ek":
		case "--encrypt-key":
			Encrypt.EncryptKey(fileName);
			break;
		case "-e":
		case "--encrypt":
			Encrypt.EncryptPass(fileName);
			break;
		case "-dk":
		case "--decrypt-key":
			Decrypt.DecryptKey(fileName);
			break;
		case "-d":
		case "--decrypt":
			Decrypt.DecryptPass(fileName);
			break;
		case "-h":
		case "--hash":
			Hash.HashFile(fileName);
			break;
		default:
			Console.WriteLine(help);
			break;
	}
} catch (AuthenticationTagMismatchException) {
	Console.Error.WriteLine("Error: Authentication tag mismatch - possible file corruption or incorrect key / file");
	Environment.Exit(1);
} catch (CryptographicException) {
	Console.Error.WriteLine("Error: Encryption failed)");
	Environment.Exit(1);
} catch (UnauthorizedAccessException) {
	Console.Error.WriteLine("Error: Insufficient permissions for selected file or current directory key path");
	Environment.Exit(1);
}
