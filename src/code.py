"""Entry point - So Circuit Pythony"""

import aesio
import os
from binascii import hexlify

from wiznet5keth import NetworkConfig, config_eth, wsgi_web_server
from config_utils import get_config_from_json_file


secrets = get_config_from_json_file(".secrets.json")  # Yeah lets just put all these in memory, yolo...

# WebServer config and setup
eth_interface = config_eth(NetworkConfig(**get_config_from_json_file()))
wsgi_server, web_app = wsgi_web_server(eth_interface)
wsgi_server.start()

def _crpyto_operation(key, data, method):
    # Example snippet to work from
    # key = b'Sixteen byte key'
    # inp = b'CircuitPython!!!' # Note: 16-bytes long
    # iv = os.urandom(16)

    # # Encrypt
    # outp = bytearray(len(inp))
    # cipher = aesio.AES(key, aesio.MODE_CTR, iv)
    # cipher.encrypt_into(inp, outp)
    # hexlify(outp)

    # # Decrypt
    # cipher = aesio.AES(key, aesio.MODE_CTR, iv)
    # inp = bytes(outp)
    # outp = bytearray(len(inp))
    # cipher.encrypt_into(inp, outp)
    # hexlify(outp)
    # print(outp)
    pass
