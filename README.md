# crypty
Encrypt files on the command line using AES-GCM. Automatically creates zip/tar archives (based on platform) for directories.

### to use
```
Usage: crypty [option] [filename]
Encrypts single files with AES-128 encryption.
Directories are archived with zip or tar before encryption.
    Mandatory arguments:
        -e,  --encrypt        encrypts filename to filename.enc
        -d,  --decrypt        decrypts filename to filename.dec
```

Encrypt a file using the ```-e``` switch and the file name

```
crypty -e dog.txt
```

Decrypt the file using the ```-d``` switch with the file name

```
crypty -d dog.txt.enc
```

