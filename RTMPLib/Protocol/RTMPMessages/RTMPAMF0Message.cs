using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMPLib
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
			Body.BinaryWriter.Write((byte)AMF0Types.Number);
			Body.BinaryWriter.Write(val);
		}

		/// <summary>
		/// bool
		/// </summary>
		/// <param name="val"></param>
		public void Put(bool val)
		{
			Body.BinaryWriter.Write((byte)AMF0Types.Boolean);
			if (val)
			{
				Body.BinaryWriter.Write((byte)1);
			}
			else
			{
				Body.BinaryWriter.Write((byte)0);
			}
		}

		public void Put(String val)
		{
			if (val.Length > ushort.MaxValue)
			{
				Body.BinaryWriter.Write((byte)AMF0Types.LongString);
				Body.BinaryWriter.Write((uint)val.Length);
				Body.Put(val);
			}
			else
			{
				Body.BinaryWriter.Write((byte)AMF0Types.String);
				Body.BinaryWriter.Write((ushort)val.Length);
				Body.Put(val);
			}
		}

		public void BeginObject()
		{
			Body.BinaryWriter.Write((byte)AMF0Types.ObjectBegin);
		}

		public void EndObject()
		{
			Body.BinaryWriter.Write((ushort)0); // number, name strlen 0 and then a byte 9.... pretty risky stuff
			Body.BinaryWriter.Write((byte)AMF0Types.ObjectEnd);
		}

		public void PutNull()
		{
			Body.BinaryWriter.Write((byte)AMF0Types.Null);
		}

		public void PutName(String val)
		{
			Body.BinaryWriter.Write((ushort)val.Length);
			Body.Put(val);
		}

		private static new Dictionary<string, Type> registered = new Dictionary<string, Type>();

		private static RTMPAMF0Message(){
			//TODO register subclasses
		}

		public static new RTMPAMF0Message Decode(RTMPMessage msg)
		{
			msg.Body.BinaryReader.BaseStream.Position = 0;
			msg.Body.BinaryReader.ReadByte(); // 02;
			short len = msg.Body.BinaryReader.ReadShort();//strlen
			String command = msg.Body.ReadString(len);
			Type tmp = registered[command];
			return (RTMPAMF0Message)tmp.GetConstructor(new Type[] { typeof(RTMPMessage) }).Invoke(new object[] { msg });
		}
	}
}
