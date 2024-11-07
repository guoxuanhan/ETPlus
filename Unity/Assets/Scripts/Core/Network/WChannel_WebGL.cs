#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using BestHTTP.WebSocket;

namespace ET
{
    public class WChannel: AChannel
    {
        private readonly WService Service;
        
        private WebSocket webSocket;

        private Queue<MemoryBuffer> waitSend = new();
        
        public WChannel(long id, IPEndPoint ipEndPoint, WService service)
        {
            this.Service = service;
            this.ChannelType = ChannelType.Connect;
            this.Id = id;
            
            WebSocket ws = new(new Uri($"ws://{ipEndPoint}"));

            this.RemoteAddress = ipEndPoint;

            // Subscribe to the WS events
            ws.OnOpen += OnOpen;
            ws.OnClosed += OnClosed;
            ws.OnError += OnError;
            ws.OnBinary += OnRead;

            // Start connecting to the server
            ws.Open();
        }
        
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            this.Id = 0;

            this.webSocket?.Close();
            this.webSocket = null;
        }

        public void Send(MemoryBuffer memoryBuffer)
        {
            if (this.webSocket == null)
            {
                this.waitSend.Enqueue(memoryBuffer);
                return;
            }

            SendOne(memoryBuffer);
        }

        private void SendOne(MemoryBuffer memoryBuffer)
        {
            this.webSocket.Send(memoryBuffer.GetBuffer(), (ulong)memoryBuffer.Position, (ulong)memoryBuffer.Length);
        }

        private void OnOpen(WebSocket ws)
        {
            if (ws == null)
            {
                this.OnError(ErrorCore.ERR_WebsocketConnectError);
                return;
            }

            if (this.IsDisposed)
            {
                return;
            }
                
            this.webSocket = ws;

            while (this.waitSend.Count > 0)
            {
                MemoryBuffer memoryBuffer = this.waitSend.Dequeue();
                this.SendOne(memoryBuffer);
            }
        }

        /// <summary>
        /// Called when we received a text message from the server
        /// </summary>
        private void OnRead(WebSocket ws, byte[] data)
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            MemoryBuffer memoryBuffer = this.Service.Fetch();
            memoryBuffer.Write(data);
            memoryBuffer.Seek(0, SeekOrigin.Begin);
            this.Service.ReadCallback(this.Id, memoryBuffer);
        }

        /// <summary>
        /// Called when the web socket closed
        /// </summary>
        private void OnClosed(WebSocket ws, UInt16 code, string message)
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            Log.Error($"wchannel closed: {code} {message}");
            this.OnError(0);
        }

        /// <summary>
        /// Called when an error occured on client side
        /// </summary>
        private void OnError(WebSocket ws, string error)
        {
            if (this.IsDisposed)
            {
                return;
            }
            
            Log.Error($"WChannel error: {this.Id} {ws.GetHashCode()} {error}");
            
            this.OnError(ErrorCore.ERR_WebsocketError);
        }
        
        private void OnError(int error)
        {
            Log.Info($"WChannel error: {this.Id} {error}");
            
            long channelId = this.Id;
			
            this.Service.Remove(channelId);
			
            this.Service.ErrorCallback(channelId, error);
        }
    }
}
#endif