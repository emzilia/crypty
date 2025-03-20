#!/usr/bin/env python3
import pickle, sys, os
try:
    from cryptography.fernet import Fernet
except ImportError:
    print("Error: Required pip package missing: cryptography")
    sys.exit(1)

def create_archive(folder):
    if (sys.platform == "win32"):
        try:
            archivename = folder + '.zip'
            print("Creating zip archive...")
            os.system(f"tar.exe -a -c -f {archivename} {folder}")
            return archivename
        except:
            print("Error: tar program missing or archive creation failed")
            sys.exit(1)
    else:
        try:
            archivename = folder + '.tar'
            print("Creating tar archive...")
            os.system(f"tar -cf {archivename} {folder}") 
            return archivename
        except:
            print("Error: tar package missing or archive creation failed")
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

    # The Fernet object to perform the encryption
    fer = Fernet(key)

    # Checks if file is a folder, if it is it creates an archive out of it.
    # Either way, the file gets read or it throws an error
    try:
        if os.path.isdir(filename):
            file = create_archive(filename)
            isarchive = True
        else:
            file = filename
            isarchive = False

        with open(file, 'rb') as filef:
            file = filef.read()
    except:
        print("Error: Unable to read file/directory")
        sys.exit(1)

    # Encrypts file and gives it a new name, or throws an error
    try:
        encrypted = fer.encrypt(file)
        if (isarchive): encryptedname = filename + '.tar.enc'
        else: encryptedname = filename + '.enc'
        print("File encryption in progress...")
        with open(encryptedname, 'wb') as cryptf:
            cryptf.write(encrypted)
        print(f"File encryption success: output -> {encryptedname}, {keyname}")
    except:
        print("Error: File encryption failed")
        sys.exit(1)

    # Automatically removes the tar/zip file used to create the archive.
    # Want to keep it? Create it yourself then!
    file_cleanup(filename)

def decrypt_file(filename, keyfile):
    # Loads the keyfile, if its not a valid serial object it
    # throws an error
    try:
        with open(keyfile, 'rb') as keyf:
            key = pickle.load(keyf)
    except:
        print("Error: Unable to load keyfile, either not a key or the file"
        "has been modified")
        sys.exit(1)

    # The Fernet object to perform the decryption
    fer = Fernet(key)

    # Tries to read the encrypted file, throws an error if it can't
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
        'Encrypts single files with AES-128 encryption.\nTar/zip archives '
        'are automatically created for directories.\n'
        '   Mandatory arguments:\n'
        '     -e, --encrypt     encrypts filename to filename.enc, key to '
        'filename.key\n'
        '     -d, --decrypt     decrypts filename to filename.dec using '
        'keyname\n'
    )

def file_cleanup(filename):
    archivefile = filename + '.tar'
    if os.path.isfile(archivefile):
        print("Cleaning up archive file...")
        os.remove(archivefile)

def process_arguments():
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

if __name__ == "__main__":
    process_arguments()
