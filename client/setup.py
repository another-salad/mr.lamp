import os
from setuptools import setup, find_packages

def read(fname):
    return open(os.path.join(os.path.dirname(__file__), fname)).read()

setup(
    name="mr.lamp.client",
    author="AnotherSalad",
    license="BSD",
    python_requires='>=3.9',
    packages=find_packages(),
    install_requires=read("requirements.txt"),
    entry_points=dict(
        console_scripts=[
            'encrypt = src.main:encrypt',
            'decrypt = src.main:decrypt',
            'communicate = src.main:communicate'
        ]
    ),
    include_package_data=True
)