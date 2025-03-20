#!/usr/bin/env python3
import pickle, sys, os, base64 
from getpass import getpass
try:
    from cryptography.fernet import Fernet
    from cryptography.hazmat.primitives import hashes
    from cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC
except ImportError:
    print("Error: Required pip package missing: cryptography")
    sys.exit(1)

# Generates a key using the provided salt. If no salt is provided, generates
# a new one.
class Crypty():
    is_windows = False

    def generate_passkey(self, salt):
        password = getpass("Enter the encrypted file's password: ")
        if (salt == 0):
            salt = os.urandom(16)
        kdf = PBKDF2HMAC(
            algorithm = hashes.SHA256(),
            length = 32,
            salt = salt,
            iterations = 480000,
        )
        key = base64.urlsafe_b64encode(kdf.derive(password.encode()))
        return key, salt

    # On Windows, creates a zip archive. If not, creates a tar archive
    def create_archive(self, folder):
        if self.is_windows:
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

    def encrypt_filekey(self, filename):
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
                file = self.create_archive(filename)
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
            if (isarchive): 
                if self.is_windows:
                    encryptedname = filename + '.zip.enc'
                else:
                    encryptedname = filename + '.tar.enc'
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
        self.file_cleanup(filename)

    def encrypt_filepass(self, filename):
        # Generates a key with a random salt using the password provided.
        try:
            salt = 0
            key, salt = self.generate_passkey(salt)
        except:
            print("Error: Key generation failed, check your entered password.")
            sys.exit(1)

        # The Fernet object to perform the encryption
        fer = Fernet(key)

        # Checks if file is a folder, if it is it creates an archive out of it.
        # Either way, the file gets read or it throws an error
        try:
            if os.path.isdir(filename):
                file = self.create_archive(filename)
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
            if (isarchive): 
                if self.is_windows:
                    encryptedname = filename + '.zip.enc'
                else:
                    encryptedname = filename + '.tar.enc'
            else: encryptedname = filename + '.enc'
            print("File encryption in progress...")
            with open(encryptedname, 'wb') as cryptf:
                cryptf.write(encrypted + os.urandom(8) + salt + os.urandom(8))
            print(f"File encryption success: output -> {encryptedname}")
        except:
            print("Error: File encryption failed")
            sys.exit(1)

        # Automatically removes the tar/zip file used to create the archive.
        # Want to keep it? Create it yourself then!
        self.file_cleanup(filename)


    def decrypt_filekey(self, filename, keyfile):
        # Loads the keyfile, if its not a valid serial object it throws an error
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

    def decrypt_filepass(self, filename):
        # Tries to read the encrypted file, throws an error if it can't
        try:
            with open(filename, 'rb') as filef:
                    encrypted = filef.read()
            salt = encrypted[-24:-8]
        except:
            print("Error: Unable to read encrypted file, it might be corrupted")
            sys.exit(1)

        # Slices out the salt from the end of the encrypted file
        try:
            key, salt = self.generate_passkey(salt)
        except:
            print("Error: Unknown issue when generating key from salt.")
            sys.exit(1)

        # The Fernet object to perform the decryption
        fer = Fernet(key)

        try:
            decrypted = fer.decrypt(encrypted[:-32])
            if filename[-4:] == '.enc':
                decryptedname = filename[:-4]+ '.dec'
            else:
                decryptedname = filename + '.dec'
            print("File decryption in progress...")
            with open(decryptedname, 'w') as decryptf:
                decryptf.write(decrypted.decode('utf-8'))
            print(f"File decryption success: output -> {decryptedname}")
        except:
            print("Error: File decryption failed, ensure you're entering the "
                  "correct password.")
            sys.exit(1)


    # Prints a cute little help readout
    def print_help(self):
        print(
            'Usage: crypty [option] filename (keyname)\n'
            'Encrypts single files with AES-128 encryption.\nTar/zip archives '
            'are automatically created for directories.\n'
            'Defaults to password-based encryption. To use a keyfile instead, '
            'use -ek/-dk.\n'
            '   Mandatory arguments:\n'
            '     -e,  --encrypt        encrypts filename to filename.enc with a '
            'password\n'
            '     -d,  --decrypt        decrypts filename to filename.dec using a '
            'password\n'
            '     -ek, --encryptkey     encrypts filename to filename.enc, key to '
            'filename.key\n'
            '     -dk, --decryptkey     decrypts filename to filename.dec using '
            'keyname\n'

        )

    # If you're on Windows, remove the zip file. If not, remove the tar
    def file_cleanup(self, filename):
        if self.is_windows:
            archivefile = filename + '.zip'
            if os.path.isfile(archivefile):
                print("Cleaning up archive file...")
                os.remove(archivefile)
        else:
            archivefile = filename + '.tar'
            if os.path.isfile(archivefile):
                print("Cleaning up archive file...")
                os.remove(archivefile)

    # Quick platform check
    def check_platform(self):
        if (sys.platform) == 'win32':
            self.is_windows = True

    def process_arguments(self):
        self.check_platform
        if len(sys.argv) == 1:
            self.print_help()
        elif len(sys.argv) == 2:
            if sys.argv[1] == '-h' or sys.argv[1] == '--help':
                self.print_help()
            else:
                print("Error: Mandatory argument missing, see 'crypty --help'")
        elif len(sys.argv) == 3:
            if sys.argv[1] == '-e' or sys.argv[1] == '--encrypt':
                self.encrypt_filepass(sys.argv[2])
            elif sys.argv[1] == '-ek' or sys.argv[1] == '--encryptkey':
                self.encrypt_filekey(sys.argv[2])
            elif sys.argv[1] == '-d' or sys.argv[1] == '--decrypt':
                self.decrypt_filepass(sys.argv[2])
            else:
                print("Error: Mandatory argument missing, see 'crypty --help'")
        elif len(sys.argv) == 4:
            if sys.argv[1] == '-dk' or sys.argv[1] == '--decryptkey':
                self.decrypt_filekey(sys.argv[2], sys.argv[3])
            elif sys.argv[1] == '-e' or sys.argv[1] == '--encrypt':
                print("Error: Only single-file encryption is supported,"
                      " consider an archive for multiple files.")
            else:
                print("Error: Mandatory argument missing, see 'crypty --help'")
        elif len(sys.argv) > 4:
            print("Error: Too many arguments, see 'crypty --help'")

