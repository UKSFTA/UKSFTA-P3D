import struct
import sys

def read_bool(f):
    b = f.read(1)
    if not b: raise EOFError("EOF reading bool")
    return struct.unpack('B', b)[0] != 0

def read_asciiz(f):
    res = b""
    while True:
        c = f.read(1)
        if not c: raise EOFError("EOF reading asciiz")
        if c == b"\x00": break
        res += c
    return res.decode('ascii')

def read_ulong(f):
    b = f.read(4)
    if not b: raise EOFError("EOF reading ulong")
    return struct.unpack('<I', b)[0]

f = open("test_p3ds/T7_MLOD.p3d", "rb")
# MLOD header: "MLOD" (4), Version (4), LODCount (4), P3DM (4) -> 16 bytes
f.seek(16)
sig = f.read(4)
print(f"Signature: {sig}")

while True:
    pos = f.tell()
    try:
        active = read_bool(f)
        name = read_asciiz(f)
        length = read_ulong(f)
        print(f"Read Tagg: {name}, Active: {active}, Len: {length}, Pos: {pos}")
        if name == "#EndOfFile#": break
        f.seek(length, 1) # Skip data
    except Exception as e:
        print(f"Error at {f.tell()}: {e}")
        break
f.close()
