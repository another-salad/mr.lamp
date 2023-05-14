"""Main entry point"""

from cryptography.hazmat.primitives.ciphers import Cipher, algorithms, modes
from cryptography.hazmat.backends import default_backend
import os
import json
from binascii import hexlify, unhexlify
from pathlib import Path
from typing import Union

SECRETS = json.loads(Path(Path(__file__).parent, ".secrets.json").read_bytes())

class NotStrOrBytes(Exception):
    """Normal shouty Python thing. Please give me a str or some bytes. Nothing else."""

def _make_bytes(message: Union[str, bytes]) -> bytes:
    if isinstance(message, str):
        message = message.encode()
    if not isinstance(message, bytes):
        raise NotStrOrBytes("Input should be a str or bytes.")
    return message

def _encrypt(secret: str, message: Union[str, bytes]) -> bytes:
    iv = os.urandom(16)
    cipher = Cipher(algorithms.AES(unhexlify(SECRETS[secret])), modes.CTR(iv), backend=default_backend())
    encryptor = cipher.encryptor()
    cipher_text = encryptor.update(_make_bytes(message))
    cipher_text += encryptor.finalize()
    cipher_text += iv
    return hexlify(cipher_text)

def _decrypt(secret: str, message: bytes) -> bytes:
    message = unhexlify(_make_bytes(message))
    iv = message[-16:]
    message_trimmed = message[:-16]
    cipher = Cipher(algorithms.AES(unhexlify(SECRETS[secret])), modes.CTR(iv), backend=default_backend())
    decryptor = cipher.decryptor()
    cipher_text = decryptor.update(message_trimmed) + decryptor.finalize()
    return cipher_text

def encrypt(message: Union[str, bytes]) -> bytes:
    buff_0 = _encrypt("client", message)
    return _encrypt("server", buff_0)

def decrypt(message: Union[str, bytes]) -> str:
    buff_0 = _decrypt("server", message)
    return _decrypt("client", buff_0).decode()

def communicate(endpoint: str, key: str, message: str) -> str:
    pass
