using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace UtilsLib
{
    public class ArrayUtils<T>
    {
        static public T[][] GetUnderArray(T[][] array, int x, int y, int xSize, int ySize)
        {
            T[][] newArray = new T[xSize][];
            for (int i = x; i < x + xSize; ++i)
            {
                newArray[i - x] = new T[ySize];
                for (int j = y; j < y + ySize; ++j)
                {
                    newArray[i - x][j - y] = array[i][j];
                }
            }
            return newArray;
        }
    }


    public class ObjectStreamReader
    {
        private Stream stream;
        public ObjectStreamReader(Stream s)
        {
            stream = s;
        }
        public bool EndOfStream
        {
            get
            {
                return this.stream.Position == this.stream.Length;
            }
        }
        public UInt16 ReadUInt16()
        {
            byte[] buffer = new byte[sizeof(UInt16)];
            stream.Read(buffer, 0, (Int32)sizeof(UInt16));
            return BitConverter.ToUInt16(buffer, 0);
        }
        public UInt32 ReadUInt32()
        {
            byte[] buffer = new byte[sizeof(UInt32)];
            stream.Read(buffer, 0, (Int32)sizeof(UInt32));
            return BitConverter.ToUInt32(buffer, 0);
        }
        public Int32 ReadInt32()
        {
            byte[] buffer = new byte[sizeof(Int32)];
            stream.Read(buffer, 0, (Int32)sizeof(Int32));
            return BitConverter.ToInt32(buffer, 0);
        }
        public UInt64 ReadUInt64()
        {
            byte[] buffer = new byte[sizeof(UInt64)];
            stream.Read(buffer, 0, (Int32)sizeof(UInt64));
            return BitConverter.ToUInt64(buffer, 0);
        }
        public byte ReadByte()
        {
            byte[] buffer = new byte[sizeof(byte)];
            stream.Read(buffer, 0, (Int32)sizeof(byte));
            return buffer[0];
        }
        public long SeekFromBegin(long offset)
        {
            return this.stream.Seek(offset, SeekOrigin.Begin);
        }

        public long SeekFromCurrent(long offset)
        {
            return this.stream.Seek(offset, SeekOrigin.Current);
        }
    }
    public class ObjectStreamWriter
    {
        private Stream stream;
        
        public ObjectStreamWriter(Stream stream)
        {
            this.stream = stream;
        }
        public long SeekFromBegin(long offset)
        {
            return this.stream.Seek(offset, SeekOrigin.Begin);
        }
        public void WriteByte(byte val)
        {
            stream.Write(new byte[] {val},0, (Int32)sizeof(byte));
        }
        public void WriteUInt32(UInt32 val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, (Int32)sizeof(UInt32));
        }
        public void WriteUInt16(UInt16 val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, (Int32)sizeof(UInt16));
        }

        public void WriteUInt64(UInt64 val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, (Int32)sizeof(UInt64));
        }

        public void WriteInt32(int val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, (Int32)sizeof(int));
        }
        
        public long Position
        {
            get
            {
                return this.stream.Position;
            }
        }
    }
}
