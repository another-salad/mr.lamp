"""Entry point - So Circuit Pythony"""

from wiznet5keth import NetworkConfig, config_eth, wsgi_web_server
from config_utils import get_config_from_json_file


# Network config file
eth_interface = config_eth(NetworkConfig(**get_config_from_json_file()))
# WSGI and Web App
wsgi_server, web_app = wsgi_web_server(eth_interface)
wsgi_server.start()

