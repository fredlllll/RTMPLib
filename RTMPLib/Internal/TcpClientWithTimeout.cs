﻿/**********************************************************************************************
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
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace RTMPLib.Internal
{
    public class TcpClientWithTimeout
    {
        public TcpClient InternalClient
        {
            get;
            private set;
        }

        private volatile bool connected = false;
        public bool Connected
        {
            get { return connected; }
            private set { connected = value; }
        }

        public IPEndPoint EndPoint
        {
            get;
            private set;
        }
        protected Exception exception;

        public TcpClientWithTimeout(IPEndPoint ipe)
        {
            EndPoint = ipe;
        }

        public void Connect(int timeoutMilliseconds = 2000)
        {
            Connected = false;
            exception = null;
            Thread connectorThread = new Thread(TryConnect)
            {
                IsBackground = true // So that a failed connection attempt wont prevent the process from terminating while it does a long timeout
            };
            connectorThread.Start();

            // wait for the thread to finish
            connectorThread.Join(timeoutMilliseconds);

            if(Connected)
            {
                // it succeeded
                return;
            }
            if(exception != null)
            {
                // it crashed
                throw exception;
            }
            else
            {
                // it timed out
                connectorThread.Interrupt();
                throw new TimeoutException("TcpClient connection to " + EndPoint.Address + ":" + EndPoint.Port + " timed out");
            }
        }

        protected void TryConnect()
        {
            try
            {
                var client = new TcpClient();
                client = new TcpClient();
                client.Connect(EndPoint);
                InternalClient = client;
                Connected = true;// record that it succeeded, for the main thread to return to the caller
            }
            catch(ThreadInterruptedException)
            {
                //just end
            }
            catch(Exception ex)
            {
                // record the exception for the main thread to re-throw back to the calling code
                exception = ex;
            }
        }
    }
}
