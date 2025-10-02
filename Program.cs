// See https://aka.ms/new-console-template for more information
//using System;
//using System.Text;
//using System.Security.Cryptography;

string help = 
@"Usage: crypty [operand] [filename]
Encrypts single files with AES-GCM encryption using a keyfile.
Directories are archived with zip or tar before encryption.
	Operands:
		-e, --encrypt	encrypts filename to filename.enc
		-d, --decrypt	decrypts filename to filename.dec";

if (args.Length != 2) {
	Console.WriteLine(help);
	Environment.Exit(1);
}

string operand = args[0];
string fileName = args[1];

static string CheckIfDir(string fileName)
{
	if (File.Exists(fileName)) {
		return fileName;
	} else if (Directory.Exists(fileName)) {
		fileName = Encrypt.CreateArchive(fileName);
		return fileName;
	} else {
		Console.WriteLine("Error: Invalid file / directory");
		Environment.Exit(1);
		return fileName;
	}
}

fileName = CheckIfDir(fileName);

switch(operand)
{
	case "-e":
	case "--encrypt":
		Encrypt.EncryptKey(fileName);
		break;
	case "-d":
	case "--decrypt":
		Decrypt.DecryptKey(fileName);
		break;
	default:
		Console.WriteLine(help);
		break;
}
