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

namespace RTMPLib
{
	public class RTMPUserControlMessage : RTMPMessage
	{

		/// <summary>
		/// short 16 bits
		/// </summary>
		public enum UserControlMessage_EventType : ushort
		{
			/// <summary>
			/// Stream begin with stream id as uint
			/// </summary>
			StreamBegin = 0x0000,
			/// <summary>
			/// stream ended
			/// </summary>
			StreamEOF = 0x0001,
			/// <summary>
			///  length in ms as double
			/// </summary>
			SetBufferLength = 0x0003,
			/// <summary>
			/// streamID as uint
			/// </summary>
			StreamIsRecorded = 0x0004,
			/// <summary>
			/// 4 byte timestamp
			/// </summary>
			PingRequest = 0x0006,
			/// <summary>
			/// 4 byte timestamp
			/// </summary>
			PingResponse = 0x0007,
		}

		public UserControlMessage_EventType EventType
		{
			get;
			private set;
		}

		public RTMPUserControlMessage(RTMPConnection connection, UserControlMessage_EventType eventType) : base (connection)
		{
			EventType = eventType;
			Header.MessageTypeID = RTMPMessageTypeID.UserControlMessage;
			Body.BinaryWriter.Write((ushort)eventType);
		}

		public RTMPUserControlMessage(RTMPMessage msg)
			: base(msg)
		{
			Body.BinaryReader.BaseStream.Position = 0;
			EventType = (UserControlMessage_EventType)Body.BinaryReader.ReadUShort();
		}

		private static new Dictionary<UserControlMessage_EventType, Type> registered = new Dictionary<UserControlMessage_EventType, Type>();
		private static RTMPUserControlMessage()
		{
			//TODO: register sub messages
		}

		public static new RTMPMessage Decode(RTMPMessage msg)
		{
			msg.Body.BinaryReader.BaseStream.Position = 0;
			UserControlMessage_EventType eventType = (UserControlMessage_EventType)msg.Body.BinaryReader.ReadUShort();
			Type tmp = registered[eventType];
			return (RTMPUserControlMessage)tmp.GetConstructor(new Type[] { typeof(RTMPMessage) }).Invoke(new object[] { msg });
		}
	}
}