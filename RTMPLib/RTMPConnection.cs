/**********************************************************************************************
 * C# RTMPLib - Version 1.0.0
 * by Frederik Gelder <frederik.gelder@freenet.de>
 *
 * Copyright 2018 Frederik Gelder
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
using System.Net;
using System.Net.Sockets;
using RTMPLib.Protocol;
using RTMPLib.Internal;
using System.Threading;

namespace RTMPLib
{
    public class RTMPConnection
    {
        public TcpClient InternalTcpClient { get; private set; }

        public CountingStream CoutingStream { get; private set; }

        private BinaryWriter bw;
        public BinaryWriter InternalBinaryWriter { get { return bw; } private set { bw = value; } }

        private BinaryReader br;
        public BinaryReader InternalBinaryReader { get { return br; } private set { br = value; } }

        public int IncomingChunkSize { get; private set; } = 128;

        public int OutgoingChunkSize { get; private set; } = 128;

        public delegate void MessageReceivedHandler(RTMPMessage message);
        public event MessageReceivedHandler MessageReceived;

        private Thread readerThread;

        private IPEndPoint remoteEndPoint;

        public RTMPConnection(IPEndPoint remoteEndPoint)
        {
            this.remoteEndPoint = remoteEndPoint;
        }

        public RTMPConnection(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {
        }

        public void Connect(int timeoutMilliseconds = 2000)
        {
            TcpClientWithTimeout tcpClient = new TcpClientWithTimeout(remoteEndPoint);
            tcpClient.Connect(timeoutMilliseconds);
            InternalTcpClient = tcpClient.InternalClient;

            CoutingStream = new CountingStream(InternalTcpClient.GetStream());
            InternalBinaryReader = new BinaryReader(CoutingStream);
            InternalBinaryWriter = new BinaryWriter(CoutingStream);

            readerThread = new Thread(ReaderRun);
            readerThread.IsBackground = true;
            readerThread.Start();
        }

        private void ReaderRun()
        {
            try
            {
                while(true)
                {
                    RTMPMessage msg = ReceiveNextMessage();
                    if(msg != null)
                    {
                        msg = RTMPMessage.Decode(msg);
                        if(MessageReceived != null)
                        {
                            MessageReceived(msg);
                        }
                    }
                }
            }
            catch
            {
                //lalala i dont care bye bye
            }
        }

        public void DoHandshake(Handshake handshake)
        {
            handshake.Do(bw, br);
        }

        private Dictionary<int, RTMPChunkStream> chunkStreams = new Dictionary<int, RTMPChunkStream>();
        public RTMPChunkStream GetChunkStream(int chunkStreamID)
        {
            RTMPChunkStream cs = null;
            if(!chunkStreams.TryGetValue(chunkStreamID, out cs))
            {
                cs = new RTMPChunkStream(this);
                chunkStreams[chunkStreamID] = cs;
            }
            return cs;
        }

        private RTMPMessage ReceiveNextMessage()
        {
            RTMPMessageHeader header = new RTMPMessageHeader(this, br);

            RTMPMessageBody body = new RTMPMessageBody(this, header, br);

            RTMPMessage newmessage = new RTMPMessage(this, header, body);

            RTMPChunkStream csinfo = GetChunkStream((int)header.ChunkStreamID);
            return csinfo.AddFragment(newmessage);
        }

        public void SendMessage(RTMPMessage message)
        {
            //TODO: if neccesary split message in smaller chunks to accomodate chunksize
            throw new NotImplementedException();
        }
    }
}
