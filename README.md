# crypty
Encrypt files using the Fernet Python module (AES-128) on the command line

#### dependencies   
Requires the cryptography package from pip (```pip install --user cryptography```) or the python-cryptography package from your system repository.   

### to install
#### on Linux   
Clone the repo with ```git clone https://github.com/emzilia/crypty```   
Read the ```install.sh``` file to see what it's doing to your system.   
Run the file with ```sh install.sh```    
The ```install.sh``` can be run a second time to remove the file from your ```$HOME/.local/bin```
  
#### on Windows   
Click on the green Code button, then the 'Download ZIP' button. Extract the files somewhere.  
Navigate to the extracted folder. Within it, blindly double-click on ```install.bat``` to 'install' the program.  
This copies the files required to run the program into the same directory Windows Store programs are kept, which is a hacky solution that should work for most usecases.   



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

You'll be prompted to create a password for the file. Afterwards you'll have the encrypted file as well as the original. If using key-based encryption, you'll also have a key file which you'll need to decrypt the file and if it's lost, the encrypted file will be unrecoverable (unless...)    
Decrypt the file using the ```-d``` switch with both the file name and optionally the keyfile name.   

```
crypty -d dog.txt.enc dog.txt.key
```

