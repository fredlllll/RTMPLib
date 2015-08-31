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

namespace RTMPLib.Protocol
{
	public class Handshake
	{
		public byte[] S0, S1, S2, C0, C1, C2;

		public void Do(BinaryWriter bw, BinaryReader br)
		{
			PrepareC0();
			SendC0(bw);
			PrepareC1();
			SendC1(bw);
			ReceiveS0(br);
			ReceiveS1(br);
			ReceiveS2(br);
			PrepareC2();
			SendC2(bw);
		}

		protected virtual void PrepareC0()
		{
			C0 = new byte[] { 3 };
		}

		protected virtual void SendC0(BinaryWriter bw)
		{
			bw.Write(C0); // C0 = Version of protocol
		}

		protected virtual void PrepareC1()
		{
			C1 = new byte[1536];

			//hardtunes specific. use override to fix this with subclass
			/*C1[4] = 0x80; // nope hardtunes wants this instead of zero
			C1[5] = 0x00;
			C1[6] = 0x07;
			C1[7] = 0x02;*/
			for (int i = 8; i < 1536; i++)
			{
				C1[i] = (byte)(i % 256);  // C1
			}
		}

		protected virtual void SendC1(BinaryWriter bw)
		{
			bw.Write(C1); // C1 is some random garbage (usually)
			bw.Flush();
		}

		protected virtual void ReceiveS0(BinaryReader br)
		{
			S0 = new byte[] { br.ReadByte() };
		}

		protected virtual void ReceiveS1(BinaryReader br)
		{
			S1 = br.ReadBytes(1536); // S1
		}

		protected virtual void ReceiveS2(BinaryReader br)
		{
			S2 = br.ReadBytes(1536); // S2 (copy of C1)
		}

		protected virtual void PrepareC2()
		{
			C2 = new byte[1536];
			for (int i = 8; i < 1536; i++) // first 8 stay 0
			{
				C2[i] = S1[i];
			}
		}

		protected virtual void SendC2(BinaryWriter bw)
		{
			bw.Write(C2);  // C2 (copy of S1)
		}
	}
}
