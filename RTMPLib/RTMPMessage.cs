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
using System.Reflection;
using RTMPLib.Internal;
using RTMPLib.Messages;

namespace RTMPLib
{
	public class RTMPMessage
	{
		public RTMPConnection Connection
		{
			get;
			private set;
		}
		public RTMPMessageHeader Header
		{
			get;
			private set;
		}

		public RTMPMessageBody Body
		{
			get;
			private set;
		}

		public RTMPMessage(RTMPConnection connection)
		{
			Connection = connection;
			Header = new RTMPMessageHeader(this);
			Body = new RTMPMessageBody(this);
		}

		public void Send(BinaryWriter bw)
		{
			Header.Send(bw);
			Body.Send(bw);
		}

		public RTMPMessage(RTMPConnection connection, RTMPMessageHeader header, RTMPMessageBody body)
		{
			Connection = connection;
			Header = header;
			header.ParentMessage = this;
			Body = body;
			body.ParentMessage = this;
		}

		protected RTMPMessage(RTMPMessage msg)
		{
			Connection = msg.Connection;
			Header = msg.Header;
			Body = msg.Body;
		}

		private static Dictionary<RTMPMessageTypeID, Type> registered = new Dictionary<RTMPMessageTypeID, Type>();
		private static Dictionary<RTMPMessageTypeID, MethodInfo> registeredDecodeMethods = new Dictionary<RTMPMessageTypeID, MethodInfo>();

		private static Type[] constructorTypes = new Type[] { typeof(RTMPMessage) };

		static RTMPMessage()
		{
			RegisterType(typeof(RTMPAcknowledgement), RTMPMessageTypeID.Acknowledgement);
			RegisterType(typeof(RTMPAMF0Message), RTMPMessageTypeID.AMF0);
			RegisterType(typeof(RTMPAudioMessage), RTMPMessageTypeID.AudioPacket);
			RegisterType(typeof(RTMPSetChunkSize), RTMPMessageTypeID.SetChunkSize);
			RegisterType(typeof(RTMPUserControlMessage), RTMPMessageTypeID.UserControlMessage);

			//TODO: add more registered
		}

		private static void RegisterType(Type t, RTMPMessageTypeID typeID)
		{
			MethodInfo d = t.GetMethod("Decode", BindingFlags.Static | BindingFlags.Public);
			registered[typeID] = t;
			registeredDecodeMethods[typeID] = d;
		}

		/// <summary>
		/// Decode this message to the correct type
		/// </summary>
		/// <returns></returns>
		public static RTMPMessage Decode(RTMPMessage msg)
		{
			Type type = registered[msg.Header.MessageTypeID];
			MethodInfo method = registeredDecodeMethods[msg.Header.MessageTypeID];
			if (method != null)
			{
				return (RTMPMessage)method.Invoke(null, new object[] { msg });
			}
			else
			{
				ConstructorInfo constructor = type.GetConstructor(constructorTypes);
				return (RTMPMessage)constructor.Invoke(new object[] { msg });
			}
		}
	}
}
