/**********************************************************************************************
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
using RTMPLib.Internal;

namespace RTMPLib.Messages
{
	public class RTMPAMF0Message : RTMPMessage
	{
		/// <summary>
		/// byte
		/// </summary>
		public enum AMF0Types : byte
		{
			/// <summary>
			/// double
			/// </summary>
			Number = 0x00, // double 64
			/// <summary>
			/// byte, 0 = false, 1 = true
			/// </summary>
			Boolean = 0x01, // byte 0 or 1
			/// <summary>
			/// 16-bit integer string length with UTF-8 string
			/// </summary>
			String = 0x02, // (16-bit integer string length with UTF-8 string)
			/// <summary>
			/// Set of key/value pairs
			/// </summary>
			ObjectBegin = 0x03, // (Set of key/value pairs)
			Null = 0x05,
			/// <summary>
			/// 32-bit entry count
			/// </summary>
			ECMAArray = 0x08, // (32-bit entry count)
			/// <summary>
			/// preceded by an empty 16-bit int (0x0000)
			/// </summary>
			ObjectEnd = 0x09, //(preceded by an empty 16-bit string length)
			/// <summary>
			/// 32-bit entry count
			/// </summary>
			StrictArray = 0x0a, //(32-bit entry count)
			/// <summary>
			/// Encoded as IEEE 64-bit double-precision floating point number with 16-bit integer timezone offset
			/// </summary>
			Date = 0x0b, //(Encoded as IEEE 64-bit double-precision floating point number with 16-bit integer timezone offset)
			/// <summary>
			/// 32-bit integer string length with UTF-8 string
			/// </summary>
			LongString = 0x0c, //(32-bit integer string length with UTF-8 string)
			/// <summary>
			/// 32-bit integer string length with UTF-8 string
			/// </summary>
			XMLDocument = 0xf0, //(32-bit integer string length with UTF-8 string)
			/// <summary>
			/// 16-bit integer name length with UTF-8 name, followed by entries
			/// </summary>
			TypedObjectBegin = 0x10, //(16-bit integer name length with UTF-8 name, followed by entries)
			SwitchToAMF3 = 0x11,
		}

		public RTMPAMF0Message(RTMPMessage rawMessage)
			: base(rawMessage)
		{

		}

		public RTMPAMF0Message(RTMPConnection connection)
			: base(connection)
		{
			Header.MessageTypeID = RTMPMessageTypeID.AMF0;
		}

		/// <summary>
		/// Number
		/// </summary>
		/// <param name="val"></param>
		public void Put(double val)
		{
			Body.MemoryWriter.Write((byte)AMF0Types.Number);
			Body.MemoryWriter.Write(val);
		}

		/// <summary>
		/// bool
		/// </summary>
		/// <param name="val"></param>
		public void Put(bool val)
		{
			Body.MemoryWriter.Write((byte)AMF0Types.Boolean);
			if (val)
			{
				Body.MemoryWriter.Write((byte)1);
			}
			else
			{
				Body.MemoryWriter.Write((byte)0);
			}
		}

		public void Put(String val)
		{
			if (val.Length > ushort.MaxValue)
			{
				Body.MemoryWriter.Write((byte)AMF0Types.LongString);
				Body.MemoryWriter.Write((uint)val.Length);
				Body.Put(val);
			}
			else
			{
				Body.MemoryWriter.Write((byte)AMF0Types.String);
				Body.MemoryWriter.Write((ushort)val.Length);
				Body.Put(val);
			}
		}

		public void BeginObject()
		{
			Body.MemoryWriter.Write((byte)AMF0Types.ObjectBegin);
		}

		public void EndObject()
		{
			Body.MemoryWriter.Write((ushort)0); // number, name strlen 0 and then a byte 9.... pretty risky stuff
			Body.MemoryWriter.Write((byte)AMF0Types.ObjectEnd);
		}

		public void PutNull()
		{
			Body.MemoryWriter.Write((byte)AMF0Types.Null);
		}

		public void PutName(String val)
		{
			Body.MemoryWriter.Write((ushort)val.Length);
			Body.Put(val);
		}

		private static Dictionary<string, Type> registered = new Dictionary<string, Type>();

		static RTMPAMF0Message(){
			//TODO register subclasses
		}

		public static new RTMPAMF0Message Decode(RTMPMessage msg)
		{
			msg.Body.MemoryReader.BaseStream.Position = 0;
			msg.Body.MemoryReader.ReadByte(); // should be 02;
			short len = msg.Body.MemoryReader.ReadShort();//strlen
			String command = msg.Body.ReadString(len);
			Type tmp = registered[command];
			return (RTMPAMF0Message)tmp.GetConstructor(new Type[] { typeof(RTMPMessage) }).Invoke(new object[] { msg });
		}
	}
}
