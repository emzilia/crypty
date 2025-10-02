// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

class Hash
{
	public static void HashFile(string fileName)
	{
		string plaintextPath = fileName;
		byte[] hash = new byte[256];

		if (!File.Exists(fileName)) {
			Console.Error.WriteLine("Error: Invalid file");
			Environment.Exit(1);
		}

		var plaintextBytes = File.ReadAllBytes(fileName);

		using var sha3 = SHA3_256.Create();	
		
		hash = sha3.ComputeHash(plaintextBytes);

		Console.WriteLine(Convert.ToHexString(hash).ToLower());

		Environment.Exit(0);
	}
}
