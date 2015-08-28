using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RTMPLib.UNUSED
{
	public interface IReader
	{
		Stream BaseStream
		{
			get;
		}
		Endianness OutputEndianness
		{
			get;
			set;
		}
		Endianness InputEndianness
		{
			get;
			set;
		}
		byte[] ReadBytes(int count);
		byte ReadByte();
		sbyte ReadSbyte();
		ushort ReadUshort();
		short ReadShort();
		uint ReadUInt();
		int ReadInt();
		ulong ReadULong();
		long ReadLong();
		float ReadFloat();
		double ReadDouble();
	}
}
