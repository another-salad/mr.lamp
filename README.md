# mr.lamp  
Shoving a pico into a novelty lamp  

![image](https://user-images.githubusercontent.com/48966874/230769467-5e9ed6e0-884a-4758-9286-cd1b586860db.png)
  
# Details  
Circuit Python 8.0.5 - Wiznet W5100S-EVB-Pico/W5500-EVB-Pico  
  
Dependencies:  
    PICO  
    - adafruit_wiznet5k  
    - adafruit_bus_device  
    - adafruit_requests.mpy  
    - circuit-python-utils  
        - networking\generate_mac_addr.py (use to generate mac, do not place in PICO file system)  
        # --- DO NOT INCLUDE PARENT DIR WHEN COPYING TO THE PICO'S 'libs' DIR --- #  
        - utils\config_utils.py  
        - w5100s-evb-pico\*.py (all files)  
        # --- --- #  
  