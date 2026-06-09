import struct

def read_bool(f):
    b = f.read(1)
    if not b: raise EOFError("EOF")
    return struct.unpack('B', b)[0] != 0

def read_asciiz(f):
    res = b""
    while True:
        c = f.read(1)
        if not c: raise EOFError("EOF")
        if c == b"\x00": break
        res += c
    return res.decode('ascii')

def read_ulong(f):
    b = f.read(4)
    if not b: raise EOFError("EOF")
    return struct.unpack('<I', b)[0]

def read_float(f):
    return struct.unpack('<f', f.read(4))[0]

f = open("test_p3ds/T7_MLOD.p3d", "rb")
# MLOD header: "MLOD" (4), Version (4), LODCount (4)
f.seek(4) 
version = read_ulong(f)
count_lods = read_ulong(f)
print(f"MLOD Version: {version}, LODs: {count_lods}")

for i in range(count_lods):
    print(f"LOD {i} at {f.tell()}")
    # P3DM_LOD: "P3DM" (4), HeaderSize (4), Version (4), NPoints (4), NNormals (4), NFaces (4), Flags (4)
    sig = f.read(4)
    headerSize = read_ulong(f)
    version = read_ulong(f)
    nPoints = read_ulong(f)
    nNormals = read_ulong(f)
    nFaces = read_ulong(f)
    flags = read_ulong(f)
    print(f"  P3DM: {sig}, NPoints: {nPoints}, NNormals: {nNormals}, NFaces: {nFaces}")
    
    # Vertices (16 bytes * nPoints)
    f.seek(nPoints * 16, 1)
    # Normals (12 bytes * nNormals)
    f.seek(nNormals * 12, 1)
    # Faces (4 + count_sides * 16) * nFaces
    for f_idx in range(nFaces):
        count_sides = read_ulong(f)
        f.seek(count_sides * 16, 1)
        # Skip padding if needed? (Extension seems to handle it by read_face)
        if count_sides < 4:
            f.seek(16, 1)
    
    # Taggs
    tagg_sig = f.read(4)
    print(f"  Taggs signature: {tagg_sig}")
    
    while True:
        active = read_bool(f)
        name = read_asciiz(f)
        length = read_ulong(f)
        print(f"  Read Tagg: {name}, Active: {active}, Len: {length}")
        if name == "#EndOfFile#": break
        f.seek(length, 1)
    
    # Resolution (4)
    res = read_float(f)
    print(f"  Resolution: {res}")
    
f.close()
