#!/usr/bin/env python3
import pickle, sys
try:
    from cryptography.fernet import Fernet
except ImportError:
    print("Error: Required pip package missing: cryptography")
    sys.exit(1)

def encrypt_file(filename):
    # Generates a key and dumps it to 'filename.key', otherwise throws an error 
    try:
       key = Fernet.generate_key()
       keyname = filename + '.key'
       with open(keyname, 'wb') as keyf:
           pickle.dump(key, keyf)
    except:
        print("Error: Keyfile generation failed, ensure you have write "
              "permission within the current directory")
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
        print("File encryption in progress...")
        with open(encryptedname, 'wb') as cryptf:
            cryptf.write(encrypted)
        print(f"File encryption success: output -> {encryptedname}, {keyname}")
    except:
        print("Error: File encryption failed")
        sys.exit(1)


def decrypt_file(filename, keyfile):
    try:
        with open(keyfile, 'rb') as keyf:
            key = pickle.load(keyf)
    except:
        print("Error: Unable to load keyfile, either not a key or the file"
        "has been modified")
        sys.exit(1)

    fer = Fernet(key)

    try:
        with open(filename, 'rb') as filef:
                encrypted = filef.read()
    except:
        print("Error: Unable to read encrypted file, it might be corrupted")
        sys.exit(1)

    try:
        decrypted = fer.decrypt(encrypted)
        if filename[-4:] == '.enc':
            decryptedname = filename[:-4]+ '.dec'
        else:
            decryptedname = filename + '.dec'
        print("File decryption in progress...")
        with open(decryptedname, 'wb') as decryptf:
            decryptf.write(decrypted)
        print(f"File decryption success: output -> {decryptedname}")
    except:
        print("Error: File decryption failed, make sure you're using the "
              "correct keyfile")
        sys.exit(1)

def print_help():
    print(
        'Usage: crypty [option] filename (keyname)\n'
        '   Mandatory arguments:\n'
        '     -e, --encrypt     encrypts filename to filename.enc, key to '
        'filename.key\n'
        '     -d, --decrypt     decrypts filename to filename.dec using '
        'keyname\n'
    )
 
if __name__ == "__main__":
    if len(sys.argv) == 1:
        print_help()
    elif len(sys.argv) == 2:
        if sys.argv[1] == '-h' or sys.argv[1] == '--help':
            print_help()
        else:
            print("Error: Mandatory argument missing, see 'crypty --help'")
    elif len(sys.argv) == 3:
        if sys.argv[1] == '-e' or sys.argv[1] == '--encrypt':
            encrypt_file(sys.argv[2])
        else:
            print("Error: Mandatory argument missing, see 'crypty --help'")
    elif len(sys.argv) == 4:
        if sys.argv[1] == '-d' or sys.argv[1] == '--decrypt':
            decrypt_file(sys.argv[2], sys.argv[3])
        elif sys.argv[1] == '-e' or sys.argv[1] == '--encrypt':
            print("Error: Only single-file encryption is supported,"
                  " consider an archive for multiple files.")
        else:
            print("Error: Mandatory argument missing, see 'crypty --help'")
    elif len(sys.argv) > 4:
        print("Error: Too many arguments, see 'crypty --help'")

