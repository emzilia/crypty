// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Security.Cryptography;

if (args.Length < 2) {
	Console.WriteLine("Error: You must specify both an operation and a file");
	Environment.Exit(1);
}

string help = 
@"Usage: crypty [operand] [filename]
Encrypts single files with AES-GCM encryption using a keyfile.
	Operands:
		-e, --encrypt	encrypts filename to filename.enc
		-d, --decrypt	decrypts filename to filename.dec";

switch(args[0])
{
	case "-e":
	case "--encrypt":
		Encrypt.EncryptKey(args[1]);
		break;
	case "-d":
	case "--decrypt":
		Decrypt.DecryptKey(args[1]);
		break;
	default:
		Console.WriteLine(help);
		break;
}
