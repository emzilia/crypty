# crypty
Encrypt files using the Fernet Python module (AES-128) on the command line

### to install
The recommended installation method is via [pipx](https://github.com/pypa/pipx), which greatly improves the UX of installing and managing python CLI apps; it can be found in the repos of most major distributions. After it's been installed, you can install crypty by executing this command within your terminal:
```pipx install git+https://github.com/emzilia/crypty.git```

### to use
```
Usage: crypty [option] filename (keyname)
Encrypts single files with AES-128 encryption.
Tar/zip archives are automatically created for directories.
Defaults to password-based encryption. To use a keyfile instead, use -ek/-dk.
    Mandatory arguments:
        -e,  --encrypt        encrypts filename to filename.enc with a password
        -d,  --decrypt        decrypts filename to filename.dec using a password
        -ek, --encryptkey     encrypts filename to filename.enc, key to filename.key
        -dk, --decryptkey     decrypts filename to filename.dec using keyname
```

Encrypt a file using the ```-e``` switch and the file name

```
crypty -e dog.txt
```

You'll be prompted to create a password for the file. Afterwards you'll have the encrypted file as well as the original. If using key-based encryption, you'll also have a key file which you'll need to decrypt the file and if it's lost, the encrypted file will be unrecoverable    
Decrypt the file using the ```-d``` switch with both the file name and optionally the keyfile name.   

```
crypty -d dog.txt.enc dog.txt.key
```

