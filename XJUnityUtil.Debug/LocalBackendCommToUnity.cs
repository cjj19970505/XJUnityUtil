using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static XJUnityUtil.UnityAppCommManager;

namespace XJUnityUtil.Debug
{
    public class LocalBackendCommToUnity : ICommToUnity
    {
        public event EventHandler<string> Received;

        public UnityAppCommManager UnityAppCommManager { get; }
        public Queue<Message> _UnfetchedMessageBuffer;
        private object _UnfetchedMessageBufferLock;
        private Task _ServerTask;
        public int Port { get; }

        private readonly Dictionary<Guid, Message> _WaitForResultMessages;
        
        private readonly object _WaitForResultMessagesLock;

        public void SendStringMessage(params string[] values)
        {
            _UnfetchedMessageBuffer.Enqueue(new Message(false, values));
        }

        public LocalBackendCommToUnity(UnityAppCommManager unityAppCommManager, int port)
        {
            _UnfetchedMessageBuffer = new Queue<Message>();
            _UnfetchedMessageBufferLock = new object();
            _WaitForResultMessages = new Dictionary<Guid, Message>();
            _WaitForResultMessagesLock = new object();
            Port = port;
            _ServerTask = _StartServerAsync();
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        private async Task _StartServerAsync()
        {
            //System.Diagnostics.Debug.WriteLine("FUCK YOU YOU SON OF BITCH");
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:" + Port + "/");
            HttpListenerResponse response = null;
            try
            {
                httpListener.Start();
                System.Diagnostics.Debug.WriteLine("XJ:::SERVER START IN PORT:" + Port);
                while (true)
                {

                    HttpListenerContext context = await httpListener.GetContextAsync();
                    HttpListenerRequest request = context.Request;
                    string[] segments = request.Url.Segments;
                    response = context.Response;

                    if (request.HttpMethod == "GET")
                    {
                        byte[] responseBuffer = null;
                        if (segments.Length >= 2 && segments[1] == "getmessage/")
                        {
                            UnityAppCommManager.Message[] bufferArray;
                            lock (_UnfetchedMessageBufferLock)
                            {
                                bufferArray = _UnfetchedMessageBuffer.ToArray();
                                _UnfetchedMessageBuffer.Clear();
                            }
                            StringBuilder sb = new StringBuilder();
                            foreach (var buffer in bufferArray)
                            {
                                if (buffer.ResponseNeeded)
                                {
                                    lock (_WaitForResultMessagesLock)
                                    {
                                        _WaitForResultMessages.Add(buffer.Uuid, buffer);
                                    }
                                }
                                
                                sb.Append("UUID:" + buffer.Uuid.ToString());
                                sb.Append(" ");
                                sb.Append("ResponseNeeded:" + buffer.ResponseNeeded);
                                sb.Append(" ");
                                for(int i = 0; i < buffer.ValueList.Count; i++)
                                {
                                    sb.Append("VALUE_"+i+":" + HttpUtility.UrlEncode(buffer.ValueList[i]));
                                    sb.Append(" ");
                                }
                                
                                sb.Append("/");
                                
                            }
                            var s = sb.ToString();
                            responseBuffer = Encoding.Default.GetBytes(s);
                            response.ContentLength64 = responseBuffer.LongLength;
                            
                            Stream output = response.OutputStream;
                            output.Write(responseBuffer, 0, responseBuffer.Length);
                        }
                        else
                        {
                            responseBuffer = Encoding.Default.GetBytes("FUCK");
                            response.ContentLength64 = responseBuffer.LongLength;
                            Stream output = response.OutputStream;
                            output.Write(responseBuffer, 0, responseBuffer.Length);
                        }
                    }
                    else if (request.HttpMethod == "POST")
                    {
                        StreamReader streamReader = new StreamReader(request.InputStream);
                        string requestString = await streamReader.ReadToEndAsync();
                        var fieldValueDict = _GetFieldValueDict(requestString);

                        if (fieldValueDict.ContainsKey("type"))
                        {
                            if(fieldValueDict["type"] == "Send")
                            {
                                Received?.Invoke(this, fieldValueDict["message"]);
                            }
                            else if (fieldValueDict["type"] == "Result")
                            {
                                lock (_WaitForResultMessagesLock)
                                {
                                    Guid uuid = Guid.Parse(fieldValueDict["uuid"]);
                                    _WaitForResultMessages[uuid].ResponseMessage = fieldValueDict["message"];
                                    _WaitForResultMessages[uuid].Responsed = true;
                                    
                                    
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("XJ:::SERVER FAILED");
            }
        }

        public Dictionary<string, string> _GetFieldValueDict(string request)
        {
            
            string[] ss = request.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> fieldValueDict = new Dictionary<string, string>();
            foreach(string s in ss)
            {
                var pairArray = s.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (pairArray.Length > 1)
                {
                    fieldValueDict.Add(HttpUtility.UrlDecode(pairArray[0], Encoding.UTF8), HttpUtility.UrlDecode(pairArray[1], Encoding.UTF8));
                }
                else
                {
                    fieldValueDict.Add(HttpUtility.UrlDecode(pairArray[0], Encoding.UTF8), "");
                }
                

            }
            return fieldValueDict;
        }

        public Task<string> SendStringMessageForResultAsync(params string[] values)
        {
            Message message = new Message(true, values);
            _UnfetchedMessageBuffer.Enqueue(message);
            return Task.Run<string>(() =>
            {
                while (!message.Responsed)
                {
                }
                return message.ResponseMessage;
            });
        }
    }

    
}
