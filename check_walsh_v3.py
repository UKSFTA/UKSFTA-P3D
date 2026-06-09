import struct
f = open("test_p3ds/Walsh_BNVD_D_MLOD.p3d", "rb")
f.seek(16)
sig = f.read(4)
headerSize = struct.unpack('<I', f.read(4))[0]
version = struct.unpack('<I', f.read(4))[0]
nPoints = struct.unpack('<I', f.read(4))[0]
nNormals = struct.unpack('<I', f.read(4))[0]
nFaces = struct.unpack('<I', f.read(4))[0]
print(f"Sig: {sig}, HeaderSize: {headerSize}, Version: {version}, Points: {nPoints}, Normals: {nNormals}, Faces: {nFaces}")
f.close()
