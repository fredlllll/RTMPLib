using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RTMPLib.UNUSED
{
	public interface IWriter
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
		void Write(byte[] bytes);
		void Write(byte value);
		void Write(sbyte value);
		void Write(ushort value);
		void Write(short value);
		void Write(uint value);
		void Write(int value);
		void Write(ulong value);
		void Write(long value);
		void Write(float value);
		void Write(double value);
	}
}
