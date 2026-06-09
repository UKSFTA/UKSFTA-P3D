import struct
f = open("test_p3ds/T7_MLOD.p3d", "rb")
data = f.read()
print(f"File size: {len(data)}")
for i in range(0, min(100, len(data)), 1):
    print(f"{i:04x}: {data[i]:02x} {chr(data[i]) if 32 <= data[i] < 127 else '.'}")
f.close()
