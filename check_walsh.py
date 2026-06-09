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

def read_float(f):
    return struct.unpack('<f', f.read(4))[0]

f = open("test_p3ds/Walsh_BNVD_D_MLOD.p3d", "rb")
# MLOD header: "MLOD" (4), Version (4), LODCount (4)
f.seek(4) 
version = read_ulong(f)
count_lods = read_ulong(f)
print(f"MLOD Version: {version}, LODs: {count_lods}")

for i in range(count_lods):
    print(f"LOD {i} at {f.tell()}")
    sig = f.read(4)
    headerSize = read_ulong(f)
    version = read_ulong(f)
    nPoints = read_ulong(f)
    nNormals = read_ulong(f)
    nFaces = read_ulong(f)
    flags = read_ulong(f)
    f.seek(nPoints * 16, 1)
    f.seek(nNormals * 12, 1)
    for f_idx in range(nFaces):
        count_sides = read_ulong(f)
        f.seek(count_sides * 16, 1)
        if count_sides < 4: f.seek(16, 1)
    
    tagg_sig = f.read(4)
    
    while True:
        pos = f.tell()
        try:
            active = read_bool(f)
            name = read_asciiz(f)
            length = read_ulong(f)
            print(f"  Read Tagg: {name}, Active: {active}, Len: {length}, Pos: {pos}")
            if name == "#EndOfFile#": break
            f.seek(length, 1)
        except Exception as e:
            print(f"Error at {pos}: {e}")
            break
    read_float(f)
f.close()
