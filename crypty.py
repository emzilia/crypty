#!/usr/bin/env python3
from cryptography.fernet import Fernet
import pickle, sys

def encrypt_file(filename):
    try:
       key = Fernet.generate_key()
       keyname = filename + '.key'
       with open(keyname, 'wb') as keyf:
            pickle.dump(key, keyf)
    except:
        print("Error: Keyfile generation failed")
        sys.exit(1)

    fer = Fernet(key)

    try:
        file = filename
        with open(file, 'rb') as filef:
            file = filef.read()
    except:
        print("Error: Unable to read file")
        sys.exit(1)

    try:
        encrypted = fer.encrypt(file)
        encryptedname = filename + '.enc'
        with open(encryptedname, 'wb') as cryptf:
            cryptf.write(encrypted)
        print(f"File encryption success: output -> {encryptedname}, {keyname}")
    except:
        print("Error: Encryption failed")
        sys.exit(1)


def decrypt_file(filename, keyfile):
    try:
        with open(keyfile, 'rb') as keyf:
            key = pickle.load(keyf)
    except:
        print("Error: Unable to load keyfile")
        sys.exit(1)

    fer = Fernet(key)

    try:
        with open(filename, 'rb') as filef:
                encrypted = filef.read()
    except:
        print("Error: Unable to read encrypted file")
        sys.exit(1)

    try:
        decrypted = fer.decrypt(encrypted)
        decryptedname = filename[:-4]+ '.dec'
        with open(decryptedname, 'wb') as decryptf:
            decryptf.write(decrypted)
        print(f"File decryption success: output -> {decryptedname}")
    except:
        print("Error: Decryption failed")
        sys.exit(1)

def print_help():
    print(
        'Usage: crypty [option] filename (keyname)\n'
        '   Mandatory arguments:\n'
        '     -e, --encrypt     encrypts filename to filename.enc, key to filename.key\n'
        '     -d, --decrypt     decrypts filename to filename.dec using keyname\n'
    )
 
if __name__ == "__main__":
    if len(sys.argv) < 3:
        print_help()
    elif len(sys.argv) == 3:
        if sys.argv[1] == '-e' or sys.argv[1] == '--encrypt':
            encrypt_file(sys.argv[2])
    elif len(sys.argv) == 4:
        if sys.argv[1] == '-d' or sys.argv[1] == '--decrypt':
            decrypt_file(sys.argv[2], sys.argv[3])

