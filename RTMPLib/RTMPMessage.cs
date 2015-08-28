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

		public RTMPMessage(RTMPConnection connection,BinaryReader br)
		{
			Connection = connection;
			Header = new RTMPMessageHeader(this, br);
			Body = new RTMPMessageBody(this, br);
		}

		public RTMPMessage(RTMPMessage msg)
		{
			Connection = msg.Connection;
			Header = msg.Header;
			Body = msg.Body;
		}

		private static Dictionary<RTMPMessageTypeID, Type> registered = new Dictionary<RTMPMessageTypeID, Type>();
		private RTMPMessage msg;

		private static RTMPMessage()
		{

		}

		/// <summary>
		/// Decode this message to the correct type
		/// </summary>
		/// <returns></returns>
		public static RTMPMessage Decode(RTMPMessage msg)
		{
			Type tmp = registered[msg.Header.MessageTypeID];
			//MethodInfo[] all = tmp.GetMethods();
			MethodInfo method = tmp.GetMethod("Decode");
			if (method != null)
			{
				return (RTMPMessage)method.Invoke(null, new object[] { msg });
			}
			else
			{
				ConstructorInfo constructor = tmp.GetConstructor(new Type[] { typeof(RTMPMessage) });
				return (RTMPMessage)constructor.Invoke(new object[] { msg });
			}
		}
	}
}
