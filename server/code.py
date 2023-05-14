"""Entry point - So Circuit Pythony"""

import aesio
import os
from binascii import hexlify, unhexlify
import json
import asyncio

from wiznet5keth import NetworkConfig, config_eth, wsgi_web_server
from wsgi_web_app_helpers import web_response_wrapper
from config_utils import get_config_from_json_file


secrets = get_config_from_json_file(".secrets.json")  # Yeah lets just put all these in memory, yolo...

# WebServer config and setup
eth_interface = config_eth(NetworkConfig(**get_config_from_json_file()))
wsgi_server, web_app = wsgi_web_server(eth_interface)
wsgi_server.start()
server_event = asyncio.Event()

class UnsupportedTypeException(Exception):
    """Usual shouty shouty Python"""

def _validate(data):
    if not isinstance(data, bytes):
        raise UnsupportedTypeException(f"Input data is not bytes. It is {type(data)}")

    return data

def gen_bytes(size=16):
    """Waffer thin wrapper for clarity"""
    return os.urandom(size)

def decrypt(key, message, iv_size=16):
    key = unhexlify(key)
    message = _validate(message)
    message = unhexlify(message)
    iv = message[-iv_size:]
    message = message[:-iv_size]
    cipher = aesio.AES(key, aesio.MODE_CTR, iv)
    output_buff = bytearray(len(message))
    cipher.decrypt_into(message, output_buff)
    return bytes(output_buff)

def encrypt(key, message, iv=None):
    """if IV is None, generates IV from gen_bytes."""
    key = unhexlify(key)
    message = _validate(message)
    output_buff = bytearray(len(message))
    if not iv:
        iv = gen_bytes()
    cipher = aesio.AES(key, aesio.MODE_CTR, iv)
    cipher.encrypt_into(message, output_buff)
    output_buff = bytes(output_buff) + iv
    return hexlify(output_buff)

def to_morse(data):
    pass

@web_app.route("/test", methods=["POST", "GET"])
def test_endpoint(request):
    """test endpoint"""
    return (
        "200 OK",
        [("Content-type", "application/json; charset=utf-8"), ("Connection", "close")],
        [json.dumps({"success": "0"}).encode("UTF-8")]
    )


@web_app.route("/echo", methods=["POST"])
def echo(request):
    """Echo"""
    status = "200 OK"
    resp = {"success": 1}
    client_id = secrets["clients"].get(request.headers.get("uid", None), None)
    print(request.headers)
    print(client_id)
    if client_id is None:
        status = "404 NOT FOUND"
        resp["success"] = 0
    return (
        status,
        [("Content-type", "application/json; charset=utf-8"), ("Connection", "close")],
        [json.dumps(resp).encode("UTF-8")]
    )


def run_server(wsgi_server, eth_interface):
    wsgi_server.update_poll()
    eth_interface.maintain_dhcp_lease()


async def run_async_task(callable_func, stop_event, sleep_val, *args, **kwargs):
    while not stop_event.is_set():
        callable_func(*args, **kwargs)
        await asyncio.sleep(sleep_val)


async def main(wsgi_server, eth_interface, server_event):
    """Entry point"""
    server_task = asyncio.create_task(run_async_task(run_server, server_event, 0.1, wsgi_server, eth_interface))
    await asyncio.gather(server_task)


asyncio.run(main(wsgi_server, eth_interface, server_event))