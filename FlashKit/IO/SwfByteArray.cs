using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace FlashKit.IO
{
    public class SwfByteArray
    {
        public static readonly uint Filter5 = (uint.MaxValue >> -5);
        public static readonly uint Filter7 = (uint.MaxValue >> -7);
        public static readonly uint Filter8 = (uint.MaxValue >> -8);
        public static readonly uint Filter10 = (uint.MaxValue >> -10);
        public static readonly uint Filter13 = (uint.MaxValue >> -13);
        public static readonly uint Filter16 = (uint.MaxValue >> -16);
        public static readonly uint Filter23 = (uint.MaxValue >> -23);

        private MemoryStream _stream = new MemoryStream();
        private BinaryReader _reader;
        private BinaryWriter _writer;

        private int _bitPosition;

        private static uint FloatToUInt32(float value)
        {
            return Convert.ToUInt32(value);
        }

        private static float UInt32ToFloat(uint value)
        {
            return Convert.ToSingle(value);
        }

        public static int CalculateUBBits(uint num1, uint num2, uint num3 = 0, uint num4 = 0)
        {
            return Math.Max(
                Math.Max(
                    CalculateUBBits(num1),
                    CalculateUBBits(num2)
                ),
                Math.Max(
                    CalculateUBBits(num3),
                    CalculateUBBits(num4)
                )
            );
        }

        public static int CalculateUBBits(uint number)
        {
            if (number == 0)
            {
                return 0;
            }

            
            int bits = 0;
            while (number >= 1)
            {
                number >>= 1;
                bits++;
            }

            return bits;
        }

        public static int CalculateSBBits(int num1, int num2, int num3 = 0, int num4 = 0)
        {
            return Math.Max(
                Math.Max(
                    CalculateSBBits(num1),
                    CalculateSBBits(num2)
                ),
                Math.Max(
                    CalculateSBBits(num3),
                    CalculateSBBits(num4)
                )
            );
        }

        public static int CalculateSBBits(int number)
        {
            if (number == 0)
            {
                return 1;
            }

            return CalculateUBBits((uint)(number < 0 ? ~number : number)) + 1;
        }

        public long BytePosition
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _bitPosition = 0;
                _stream.Position = value;
            }
        }

        public int BitPosition
        {
            get
            {
                return _bitPosition;
            }
            set
            {
                _bitPosition = value;
            }
        }

        public long BytesAvailable
        {
            get
            {
                return _stream.Length - _stream.Position;
            }
        }

        public long Length
        {
            get
            {
                return _stream.Length;
            }
            set
            {
                _stream.SetLength(value);
            }
        }

        public MemoryStream Stream
        {
            get
            {
                return _stream;
            }
        }

        public SwfByteArray(byte[] bytes)
        {
            _writer = null;
            if (!BitConverter.IsLittleEndian)
            {
                throw new NotSupportedException("BigEndian systems aren't supported");
            }

            _stream.Write(bytes, 0, bytes.Length);
            _stream.Position = 0;

            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);
        }

        public void AlignBytes()
        {
            if (_bitPosition != 0)
            {
                _stream.Position += 1;
                _bitPosition = 0;
            }
        }

        public void Clear()
        {
            _bitPosition = 0;

            _reader.Close();
            _reader.Dispose();
            _writer.Close();
            _writer.Dispose();
            _stream.Close();
            _stream.Dispose();

            _stream = new MemoryStream();
            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);
        }

        public void ReadBytes(ref byte[] buffer)
        {
            ReadBytes(ref buffer, 0);
        }

        public void ReadBytes(ref byte[] buffer, int length)
        {
            AlignBytes();

            if (length == 0)
            {
                length = buffer.Length;
            }

            buffer = _reader.ReadBytes(length);
        }

        public void WriteBytes(byte[] buffer)
        {
            WriteBytes(buffer, 0, 0);
        }

        public void WriteBytes(byte[] buffer, int offset)
        {
            WriteBytes(buffer, offset, 0);
        }

        public void WriteBytes(byte[] buffer, int offset, int length)
        {
            AlignBytes();
            
            if (length == 0)
            {
                length = buffer.Length;
            }

            _writer.Write(buffer, offset, length);
        }

        public bool ReadFlag()
        {
            return ReadUB(1) == 1;
        }

        public void WriteFlag(bool value)
        {
            WriteUB(1, (uint)(value ? 1 : 0));
        }


        public sbyte ReadSI8()
        {
            AlignBytes();

            return _reader.ReadSByte();
        }

        public short ReadSI16()
        {
            AlignBytes();

            return _reader.ReadInt16();
        }

        public void WriteSI16(short value)
        {
            AlignBytes();

            _writer.Write(value);
        }

        public int ReadSI32()
        {
            AlignBytes();

            return _reader.ReadInt32();
        }

        public void WriteSI32(int value)
        {
            AlignBytes();

            _writer.Write(value);
        }

        public sbyte[] ReadSI8Array(int length)
        {
            sbyte[] result = new sbyte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadSI8();
            }

            return result;
        }

        public short[] ReadSI16Array(int length)
        {
            short[] result = new short[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadSI16();
            }

            return result;
        }

        public byte ReadUI8()
        {
            AlignBytes();

            return _reader.ReadByte();
        }

        public void WriteUI8(byte value)
        {
            AlignBytes();

            _writer.Write(value);
        }

        public ushort ReadUI16()
        {
            AlignBytes();

            return _reader.ReadUInt16();
        }

        public void WriteUI16(ushort value)
        {
            AlignBytes();

            _writer.Write(value);
        }

        public uint ReadUI32()
        {
            AlignBytes();

            return _reader.ReadUInt32();
        }

        public void WriteUI32(uint value)
        {
            AlignBytes();

            _writer.Write(value);
        }

        public byte[] ReadUI8Array(int length)
        {
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUI8();
            }

            return result;
        }

        public ushort[] ReadUI16Array(int length)
        {
            ushort[] result = new ushort[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUI16();
            }

            return result;
        }

        public uint[] ReadUI24Array(int length)
        {
            uint[] result = new uint[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = (uint)(_reader.ReadUInt16() << 8 | _reader.ReadByte());
            }

            return result;
        }

        public uint[] ReadUI32Array(int length)
        {
            uint[] result = new uint[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUI32();
            }

            return result;
        }

        public float ReadFixed8()
        {
            AlignBytes();

            byte dec = _reader.ReadByte();
            float result = _reader.ReadSByte();

            return result + (float)dec / 0xff;
        }

        public void WriteFixed8(float value)
        {
            AlignBytes();

            sbyte integer = (sbyte)Math.Floor(value);
            byte dec = (byte)((value - integer) * 0xff);

            _writer.Write(dec);
            _writer.Write(integer);
        }

        public float ReadFixed16()
        {
            AlignBytes();

            ushort dec = _reader.ReadUInt16();
            float result = _reader.ReadInt16();

            return result + (float)dec / 0xffff;
        }

        public void WriteFixed16(float value)
        {
            AlignBytes();

            short integer = (sbyte)Math.Floor(value);
            ushort dec = (byte)((value - integer) * 0xffff);

            _writer.Write(dec);
            _writer.Write(integer);
        }

        public float ReadFloat16()
        {
            ushort raw = ReadUI16();

            int sign = raw >> 15;
            byte exp = (byte)((raw >> 10) & Filter5);
            short sig = (short)(raw & Filter10);

            if (exp == 31)
            {
                exp = 255;
            }
            else if (exp == 0)
            {
                exp = 0;
                sig = 0;
            }
            else
            {
                exp += 111;
            }

            uint temp = (uint)(sign << 31 | exp << 23 | sig << 13);

            return UInt32ToFloat(temp);
        }

        public void WriteFloat16(float value)
        {
            uint raw = FloatToUInt32(value);

            uint sign = raw >> 31;
            uint exp = (raw >> 23) & Filter8;
            uint sig = (raw >> 13) & Filter10;

            if (exp == 255)
            {
                exp = 31;
            }
            else if (exp < 111)
            {
                exp = 0;
                sig = 0;
            }
            else if (exp > 141)
            {
                exp = 31;
                sig = 0;
            }
            else{
                exp -= 111;
            }

            ushort temp = (ushort)(sign << 15 | exp << 10 | sig);

            WriteUI16(temp);
        }

        public float ReadFloat()
        {
            return _reader.ReadSingle();
        }

        public void WriteFloat(float value)
        {
            _writer.Write(value);
        }

        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        public void WriteDouble(double value)
        {
            _writer.Write(value);
        }

        public uint ReadEncodedUI32()
        {
            AlignBytes();

            uint result = 0;
            int bytesRead = 0;

            bool shouldContinue = true;

            while (shouldContinue && bytesRead < 5)
            {
                byte currentByte = _reader.ReadByte();
                result = ((currentByte & Filter7) << (7 * bytesRead)) | result;
                shouldContinue = (currentByte >> 7) == 1;
                bytesRead += 1;
            }

            return result;
        }

        public void WriteEncodedUI32(uint value)
        {
            uint remaining = value;
            int bytesWritten = 0;
            byte currentByte;

            bool shouldContinue = true;
        
            while(shouldContinue && bytesWritten < 5)
            {
                currentByte = (byte)(remaining & Filter7);

                remaining = remaining >> 7;

                if (remaining > 0)
                {
                    currentByte = (byte)(currentByte | (1 << 7));
                }

                _writer.Write(currentByte);

                shouldContinue = remaining > 0;
                bytesWritten += 1;
            }
        }

        public uint ReadUB(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            int totalBytes = (int)Math.Ceiling(((double)_bitPosition + length) / 8);

            int iter = 0;
            uint result = 0;

            while (iter < totalBytes)
            {
                byte currentByte = _reader.ReadByte();
                result = (result << 8) | currentByte;
                iter++;
            }

            int newBitPosition = (_bitPosition + length) % 8;

            int excessBits = totalBytes * 8 - (_bitPosition + length);
           
            result = result >> excessBits;
            result = result & (uint.MaxValue >> -length);

            _bitPosition = newBitPosition;
            if (_bitPosition > 0)
            {
                _stream.Position--;
            }

            return result;
        }

        public void WriteUB(int length, uint value)
        {
            if (length == 0)
            {
                return;
            }

            int totalBytes = (int)Math.Ceiling(((double)_bitPosition + length) / 8);
            int iter = 0;
            byte currentByte;
            uint existing = 0;
            long startPosition = _stream.Position;

            while (iter < totalBytes)
            {
                currentByte = (BytesAvailable >= 1) ? _reader.ReadByte() : (byte)0;
                existing = (existing << 8) | currentByte;
                iter++;
            }

            int newBitPosition = (_bitPosition+ length) % 8;
            
            uint result = existing >> (totalBytes * 8 - _bitPosition);
            result = result << length;
            result = result | (value & (uint.MaxValue >> -length));

            int excessBits = totalBytes * 8 - (_bitPosition + length);

            if (excessBits > 0)
            {
                result = result << excessBits;
                result = result | (existing & (uint.MaxValue >> -excessBits));
            }

            _stream.Position = startPosition;

            iter = totalBytes - 1;
            
            while (iter >= 0)
            {
                _stream.Position = startPosition + iter;
                currentByte = (byte)(result & Filter8);
                result = result >> 8;

                _writer.Write((sbyte)currentByte);

                iter -= 1;
            }            

            _stream.Position = startPosition + totalBytes;
            _bitPosition = newBitPosition;

            if (_bitPosition > 0)
            {
                _stream.Position -= 1;
            }
        }

        public int ReadSB(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            int result = (int)ReadUB(length);
            int leadingDigit = (int) (((uint)result) >> (length - 1));
            
            if (leadingDigit == 1)
            {
                return (int) -((~result & (uint.MaxValue >> -length)) + 1);
            }
            return result;
        }

        public void WriteSB(int length, int value)
        {
            if (length == 0)
            {
                return;
            }

            if (value < 0)
            {
                WriteUB(length, (uint)(~Math.Abs(value) + 1));
            }
            else
            {
                WriteUB(1, 0);
                WriteUB(length - 1, (uint)value);
            }
        }

        public float ReadFB(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            int raw = ReadSB(length);

            int integer = raw >> 16;
            float dec = (float)(raw & Filter16) / 0xffff;

            return integer + dec;
        }

        public void WriteFB(int length, float value)
        {
            if (length == 0)
            {
                return;
            }

            int integer = (int)Math.Floor(value);
            uint dec = (uint)(Math.Round(Math.Abs(value - integer) * 0xffff)) & Filter16;

            int raw = ((integer << 16) | (int)dec);

            WriteSB(length, raw);
        }

        public string ReadString()
        {
            AlignBytes();

            int byteCount = 1;

            while (_reader.ReadByte() != 0x00)
            {
                byteCount += 1;
            }

            _stream.Position -= byteCount;

            string result = Encoding.UTF8.GetString(_reader.ReadBytes(byteCount - 1));

            _stream.Position += 1;

            return result;
        }

        public void WriteString(string value)
        {
            AlignBytes();

            byte[] temp = Encoding.UTF8.GetBytes(value);

            _writer.Write(temp, 0, temp.Length);
            _writer.Write((byte)0);
        }

        public string ReadStringWithLength(int length)
        {
            AlignBytes();

            return Encoding.UTF8.GetString(_reader.ReadBytes(length));
        }

        public void WriteStringWithLength(string value, int length)
        {
            AlignBytes();
            value = value.Substring(0, length);

            byte[] temp = Encoding.UTF8.GetBytes(value);

            _writer.Write(temp, 0, temp.Length);
        }

        public Color ReadARGB()
        {
            ushort alpha = ReadUI8();
            ushort red = ReadUI8();
            ushort green = ReadUI8();
            ushort blue = ReadUI8();

            return Color.FromArgb(alpha, red, green, blue);
        }

        public void WriteARGB(Color value)
        {
            WriteUI8(value.A);
            WriteUI8(value.R);
            WriteUI8(value.G);
            WriteUI8(value.B);
        }

        public Color ReadRGBA()
        {
            ushort red = ReadUI8();
            ushort green = ReadUI8();
            ushort blue = ReadUI8();
            ushort alpha = ReadUI8();

            return Color.FromArgb(alpha, red, green, blue);
        }

        public void WriteRGBA(Color value)
        {
            WriteUI8(value.R);
            WriteUI8(value.G);
            WriteUI8(value.B);
            WriteUI8(value.A);
        }
    }
}
