# crypty
encrypt files using the Fernet algorithm (AES-128) on the command line

### to install
First, read the ```install.sh``` file to see what it's doing to your system.   
Then, run the file with ```bash install.sh```    
The ```install.sh``` can be run a second time to remove the file from your ```$HOME/.local/bin```

### to use
```
Usage: crypty [option] filename (keyname)   
           Mandatory arguments:   
             -e, --encrypt     encrypts filename to filename.enc, key to filename.key   
             -d, --decrypt     decrypts filename to filename.dec using keyname   
```
   
```
crypty -e dog.txt
```

Upon encrypting a file, you'll have the encrypted file (as well as the original) and the key, the key is required to decrypt the file and if it's lost, the file will be unrecoverable.    

```
crypty -d dog.txt.enc dog.txt.key
```

