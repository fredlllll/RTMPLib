using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMPLib
{
	public enum Endianness
	{
		BigEndian,
		LittleEndian
	}
	public static class Helper
	{
		public static Endianness HostEndianness
		{
			get
			{
				unsafe
				{
					short tmp = 1;
					if (((byte*)&tmp)[0] == 0) // big endian
					{
						return Endianness.BigEndian;
					}
					else // little endian
					{
						return Endianness.LittleEndian;
					}
				}
			}
		}

		public static Endianness NetworkEndianness
		{
			get { return Endianness.BigEndian; }
		}
	}

	/// <summary>
	/// byte
	/// </summary>
	public enum RTMPMessageTypeID : byte
	{
		SetChunkSize = 0x01,
		AbortMessage = 0x02,
		Acknowledgement = 0x03,
		UserControlMessage = 0x04,
		//WindowAcknowledgementSize = 0x05,
		ServerBandwidth = 0x05,
		//SetPeerBandwidth = 0x06,
		ClientBandwidth = 0x06,
		AudioPacket = 0x08,
		VideoPacket = 0x09,
		//DataMessage 18 = amf0 15 = amf3
		DataMessageAMF3 = 0x0f,
		SharedObjectMessageAMF3 = 0x10,
		AMF3 = 0x11,
		DataMessageAMF0 = 0x12,
		SharedObjectMessageAMF0 = 0x13,
		AMF0 = 0x14,
		Aggregated = 0x16,
	}
	/// <summary>
	/// byte
	/// </summary>
	public enum RTMPMessageFormat : byte
	{
		/// <summary>
		/// 12 byte header (full header)
		/// </summary>
		FullHeader_12 = 0x00,
		/// <summary>
		/// 8 bytes - Like type b00 (full header), not including message stream ID (last 4 bytes).
		/// </summary>
		NoMessageId_8 = 0x01,
		/// <summary>
		/// 4 bytes - Basic header and timestamp (3 bytes) are included.
		/// </summary>
		BasicHeaderAndTime_4 = 0x02,
		/// <summary>
		/// 1 byte - only the basic header is included.
		/// </summary>
		BasicHeader_1 = 0x03,
		/// <summary>
		/// Format has not yet been determined.
		/// </summary>
		Undefined
	}

	/// <summary>
	/// int, should be bigger than 3 when set by user
	/// </summary>
	public enum RTMPMessageChunkStreamID : int
	{//0 = extended with 1 byte, 1 = extended with 2 bytes to hold id, 2 = low level message, else 3 to 63 as ids
		//0 and 1 should never be used. instead the streamid should be put in this variable
		ExtendedWith1Byte = 0x00,
		ExtendedWith2Bytes = 0x01,
		/// <summary>
		/// (2) marks a low level message
		/// </summary>
		LowLevelMessage = 0x02,
		/// <summary>
		/// Internal default value
		/// </summary>
		Undefined = -1,
	}
}
