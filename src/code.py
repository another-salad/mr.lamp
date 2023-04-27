"""Entry point - So Circuit Pythony"""

import aesio
import os
from binascii import hexlify

# from wiznet5keth import NetworkConfig, config_eth, wsgi_web_server
# from config_utils import get_config_from_json_file


# secrets = get_config_from_json_file(".secrets.json")  # Yeah lets just put all these in memory, yolo...

# WebServer config and setup
# eth_interface = config_eth(NetworkConfig(**get_config_from_json_file()))
# wsgi_server, web_app = wsgi_web_server(eth_interface)
# wsgi_server.start()

class UnsupportedTypeException(Exception):
    """Usual shouty shouty Python"""

def _validate(data):
    if not isinstance(data, bytes):
        if isinstance(data, str):
            data = data.encode()  # DO WE NEED TO ASCII/LATIN?
        else:
            raise UnsupportedTypeException(f"Input data is not a str/bytes. It is {type(data)}")

    return data

def gen_bytes(size=16):
    """Waffer thin wrapper for clarity"""
    return os.urandom(size)

def decrypt(key, message, iv_size=16):
    message = _validate(message)
    iv = message[-iv_size:]
    message = message[:-iv_size]
    cipher = aesio.AES(key, aesio.MODE_CTR, iv)
    output_buff = bytearray(len(message))
    cipher.decrypt_into(message, output_buff)
    return bytes(output_buff)

def encrypt(key, message, iv=None):
    """if IV is None, generates IV from gen_bytes."""
    message = _validate(message)
    output_buff = bytearray(len(message))
    if not iv:
        iv = gen_bytes()
    cipher = aesio.AES(key, aesio.MODE_CTR, iv)
    cipher.encrypt_into(message, output_buff)
    output_buff = bytes(output_buff) + iv
    return output_buff

def to_morse(data):
    pass
