using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMPLib.Internal
{
	public class RTMPChunkStream
	{
		public RTMPConnection Connection
		{
			get;
			set;
		}

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

		/// <summary>
		/// The amount of bytes missing to form a complete message
		/// </summary>
		public int RemainingBytes
		{
			get;
			set;
		}

		private Queue<RTMPMessage> streamFragments = new Queue<RTMPMessage>();

		public RTMPChunkStream(RTMPConnection connection)
		{
			Connection = connection;
		}

		public RTMPMessage AddFragment(RTMPMessage message)
		{
			if (message.Header.MessageLength > Connection.IncomingChunkSize)
			{
				if (streamFragments.Count == 0)
				{
					RemainingBytes = message.Header.MessageLength - Connection.IncomingChunkSize;
					MessageLength = message.Header.MessageLength;
					Timestamp = message.Header.Timestamp;
					MessageTypeID = message.Header.MessageTypeID;
					MessageStreamID = message.Header.MessageStreamID;
				}

				if (message.Header.MessageLength < 0) //followup
				{
					RemainingBytes -= Connection.IncomingChunkSize;
				}

				streamFragments.Enqueue(message);

				if (RemainingBytes <= 0) //finished message
				{
					return ProcessFragments();
				}
			}
			else
			{
				RemainingBytes = 0;
				return message;
			}
			return null;
		}

		private RTMPMessage ProcessFragments()
		{
			//TODO: look at first messages header and see if enough messages are there to fill the length. if yes return a new message with all the submessages content and dequeue them, else return null
			throw new NotImplementedException();
		}
	}
}
