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
