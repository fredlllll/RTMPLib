﻿/**********************************************************************************************
 * C# RTMPLib - Version 1.0.0
 * by Frederik Gelder <frederik.gelder@freenet.de>
 *
 * Copyright 2015 Frederik Gelder
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 **********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RTMPLib.Internal
{
	public class BinaryWriter
	{
		public Stream BaseStream
		{
			get;
			set;
		}

		private bool reverseBytes = true;
		private Endianness outputEndianness = Endianness.BigEndian;
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

		private Endianness inputEndianness = Endianness.LittleEndian;
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

		public BinaryWriter(Stream baseStream, Endianness outputEndianness = Endianness.BigEndian)
		{
			BaseStream = baseStream;
			InputEndianness = Helper.HostEndianness;
			OutputEndianness = outputEndianness;
		}


		public void Write(byte[] bytes)
		{
			BaseStream.Write(bytes, 0, bytes.Length);
		}

		public void Write(byte[] bytes, int offset, int length)
		{
			BaseStream.Write(bytes, offset, length);
		}

		public void Write(byte value)
		{
			BaseStream.WriteByte(value);
		}

		public void Flush()
		{
			BaseStream.Flush();
		}

		public void Write(sbyte value)
		{
			Write((byte)value);
		}

		public void Write(ushort value)
		{
			Write((short)value);
		}

		public unsafe void Write(short value)
		{
			byte[] bytesArray = new byte[2];
			byte* bytes = (byte*)&value;
			bytesArray[0] = bytes[0];
			bytesArray[1] = bytes[1];
			if (reverseBytes)
			{
				Array.Reverse(bytesArray);
			}
			Write(bytesArray);
		}

		public void Write(uint value)
		{
			Write((int)value);
		}

		public unsafe void Write(int value)
		{
			byte[] bytesArray = new byte[4];
			byte* bytes = (byte*)&value;
			bytesArray[0] = bytes[0];
			bytesArray[1] = bytes[1];
			bytesArray[2] = bytes[2];
			bytesArray[3] = bytes[3];
			if (reverseBytes)
			{
				Array.Reverse(bytesArray);
			}
			Write(bytesArray);
		}

		public void Write(ulong value)
		{
			Write((long)value);
		}

		public unsafe void Write(long value)
		{
			byte[] bytesArray = new byte[8];
			byte* bytes = (byte*)&value;
			bytesArray[0] = bytes[0];
			bytesArray[1] = bytes[1];
			bytesArray[2] = bytes[2];
			bytesArray[3] = bytes[3];
			bytesArray[4] = bytes[4];
			bytesArray[5] = bytes[5];
			bytesArray[6] = bytes[6];
			bytesArray[7] = bytes[7];
			if (reverseBytes)
			{
				Array.Reverse(bytesArray);
			}
			Write(bytesArray);
		}

		public unsafe void Write(float value)
		{
			byte[] bytesArray = new byte[4];
			byte* bytes = (byte*)&value;
			bytesArray[0] = bytes[0];
			bytesArray[1] = bytes[1];
			bytesArray[2] = bytes[2];
			bytesArray[3] = bytes[3];
			Write(bytesArray);
		}

		public unsafe void Write(double value)
		{
			byte[] bytesArray = new byte[8];
			byte* bytes = (byte*)&value;
			bytesArray[0] = bytes[0];
			bytesArray[1] = bytes[1];
			bytesArray[2] = bytes[2];
			bytesArray[3] = bytes[3];
			bytesArray[4] = bytes[4];
			bytesArray[5] = bytes[5];
			bytesArray[6] = bytes[6];
			bytesArray[7] = bytes[7];
			Write(bytesArray);
		}
	}
}
