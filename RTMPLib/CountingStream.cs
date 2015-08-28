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
using System.IO;

namespace RTMPLib
{
	public class CountingStream : Stream
	{
		/// <summary>
		/// The underlying Stream
		/// </summary>
		public Stream BaseStream
		{
			get;
			protected set;
		}

		/// <summary>
		/// the total number of bytes read by this instance
		/// </summary>
		public long TotalReadBytes
		{
			get;
			protected set;
		}

		/// <summary>
		/// the total number of bytes written by this instance
		/// </summary>
		public long TotalWrittenBytes
		{
			get;
			protected set;
		}

		/// <summary>
		/// this value can be adjusted by the user. it experiences the same changes as TotalReadBytes
		/// </summary>
		public long CustomReadBytes
		{
			get;
			set;
		}

		/// <summary>
		/// this value can be adjusted by the user. it experiences the same changes as TotalWrittenBytes
		/// </summary>
		public long CustomWrittenBytes
		{
			get;
			set;
		}

		public CountingStream(Stream baseStream)
		{
			this.BaseStream = baseStream;
		}

		public override bool CanRead
		{
			get { return BaseStream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return BaseStream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return BaseStream.CanWrite; }
		}

		public override void Flush()
		{
			BaseStream.Flush();
		}

		public override long Length
		{
			get { return BaseStream.Length; }
		}

		public override long Position
		{
			get
			{
				return BaseStream.Position;
			}
			set
			{
				throw new NotImplementedException("no seeking when counting");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int retval = BaseStream.Read(buffer, offset, count);
			TotalReadBytes += retval;
			CustomReadBytes += retval;
			return retval;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException("no seeking when counting");
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException("nonono dont do that!!");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			TotalWrittenBytes += count;
			CustomWrittenBytes += count;
			try
			{
				BaseStream.Write(buffer, offset, count);
			}
			catch // networkstream checks if buffer is big enough and throws exception.
			{
				TotalWrittenBytes -= count;
				CustomWrittenBytes -= count;
			}
		}
	}
}
