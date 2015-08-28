using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RTMPLib
{
	public class BinaryReader
	{
		public Stream BaseStream
		{
			get;
			set;
		}

		private bool reverseBytes = true;
		private Endianness outputEndianness = Endianness.LittleEndian;
		public Endianness OutputEndianness
		{
			get
			{
				return outputEndianness;
			}
			set
			{
				outputEndianness = value;
				reverseBytes = outputEndianness != inputEndianness;
			}
		}

		private Endianness inputEndianness = Endianness.BigEndian;
		public Endianness InputEndianness
		{
			get
			{
				return inputEndianness;
			}
			set
			{
				inputEndianness = value;
				reverseBytes = outputEndianness != inputEndianness;
			}
		}

		public BinaryReader(Stream baseStream, Endianness streamEndianness = Endianness.BigEndian)
		{
			BaseStream = baseStream;
			outputEndianness = Helper.HostEndianness;
			inputEndianness = streamEndianness;
		}

		public byte[] ReadBytes(int count)
		{
			byte[] retval = new byte[count];

			int offset = 0;
			do
			{
				int num2 = BaseStream.Read(retval, offset, count);
				offset += num2;
				count -= num2;
			}
			while (count > 0);

			return retval;
		}

		public byte ReadByte()
		{
			int retval = BaseStream.ReadByte();
			if (retval == -1)
			{
				throw new EndOfStreamException();
			}
			return (byte)retval;
		}

		public sbyte ReadSbyte()
		{
			return (sbyte)ReadByte();
		}

		public unsafe ushort ReadUShort()
		{
			return (ushort)ReadShort();
		}

		public unsafe short ReadShort()
		{
			byte[] bytesArray = ReadBytes(2);
			fixed (byte* bytes = bytesArray)
			{
				if (reverseBytes)
				{
					Array.Reverse(bytesArray);
				}
				return *(short*)bytes;

			}
		}

		public uint ReadUInt()
		{
			return (uint)ReadInt();
		}

		public unsafe int ReadInt()
		{
			byte[] bytesArray = ReadBytes(4);
			fixed (byte* bytes = bytesArray)
			{
				if (reverseBytes)
				{
					Array.Reverse(bytesArray);
				}
				return *(int*)bytes;

			}
		}

		public ulong ReadULong()
		{
			return (ulong)ReadLong();
		}

		public unsafe long ReadLong()
		{
			byte[] bytesArray = ReadBytes(8);
			fixed (byte* bytes = bytesArray)
			{
				if (reverseBytes)
				{
					Array.Reverse(bytesArray);
				}
				return *(long*)bytes;

			}
		}

		public unsafe float ReadFloat()
		{
			fixed (byte* bytes = ReadBytes(4))
			{
				return *(float*)bytes;
			}
		}

		public unsafe double ReadDouble()
		{
			fixed (byte* bytes = ReadBytes(8))
			{
				return *(double*)bytes;
			}
		}
	}
}
