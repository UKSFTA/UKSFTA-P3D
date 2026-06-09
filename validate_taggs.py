import struct
import os
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

def validate_file(filepath):
    try:
        f = open(filepath, "rb")
        # MLOD header: "MLOD" (4), Version (4), LODCount (4)
        f.seek(4) 
        version = read_ulong(f)
        count_lods = read_ulong(f)
        
        for i in range(count_lods):
            # P3DM header
            f.seek(4, 1) # P3DM
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
                active = read_bool(f)
                name = read_asciiz(f)
                length = read_ulong(f)
                
                # Check data length
                data_start = f.tell()
                f.seek(length, 1)
                data_end = f.tell()
                
                if name == "#EndOfFile#": break
        
        f.close()
        return True, "Valid"
    except Exception as e:
        return False, str(e)

files = [f for f in os.listdir("test_output_v18") if f.endswith("_MLOD.p3d")]
for file in files:
    valid, msg = validate_file(os.path.join("test_output_v18", file))
    print(f"{file}: {valid} - {msg}")
