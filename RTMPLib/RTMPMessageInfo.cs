using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMPLib
{
	/// <summary>
	/// Holds informations about the last received header values
	/// </summary>
	public class RTMPMessageInfo
	{
		/// <summary>
		/// Last recorded Timestamp
		/// </summary>
		public uint Timestamp
		{
			get;
			set;
		}

		/// <summary>
		/// Last recorded MessageLength
		/// </summary>
		public int MessageLength
		{
			get;
			set;
		}

		/// <summary>
		/// Last recorded MessageTypeID
		/// </summary>
		public RTMPMessageTypeID MessageTypeID
		{
			get;
			set;
		}

		/// <summary>
		/// Last recorded MessageStreamID
		/// </summary>
		public int MessageStreamID
		{
			get;
			set;
		}
	}
}
