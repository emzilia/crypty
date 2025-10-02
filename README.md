# crypty
Encrypt files on the command line using AES-GCM. Automatically creates zip/tar archives (based on platform) for directories.

### to use
```
Usage: crypty [operand] [filename]
Encrypts files with AES-GCM encryption with a keyfile.
Directories are archived with zip or tar before encryption.
    Operands:
        -e,  --encrypt        encrypts filename to filename.enc
        -d,  --decrypt        decrypts filename to filename.dec
        -h,  --hash           provides the SHA3-256 hash of a file
```

Encrypt a file using the ```-e``` switch and the file name

```
crypty -e dog.txt
```

Decrypt the file using the ```-d``` switch with the file name

```
crypty -d dog.txt.enc
```

