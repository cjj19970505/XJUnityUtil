using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XJUnityUtil.Debug
{
    public class LocalBackendCommToUnity : ICommToUnity
    {
        public event EventHandler<string> Received;

        public UnityAppCommManager UnityAppCommManager { get; }
        public Queue<string> _UnfetchedMessageBuffer;
        private object _UnfetchedMessageBufferLock;
        private Task _ServerTask;
        public int Port { get; }

        public void SendStringMessage(string value)
        {
            _UnfetchedMessageBuffer.Enqueue(value);
        }

        public LocalBackendCommToUnity(UnityAppCommManager unityAppCommManager, int port)
        {
            _UnfetchedMessageBuffer = new Queue<string>();
            _UnfetchedMessageBufferLock = new object();
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
                            string[] bufferArray;
                            lock (_UnfetchedMessageBufferLock)
                            {
                                bufferArray = _UnfetchedMessageBuffer.ToArray();
                                _UnfetchedMessageBuffer.Clear();
                            }
                            string s = "";
                            foreach (var buffer in bufferArray)
                            {
                                s += buffer.ToString();
                                s += '/';
                            }
                            
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
                        
                        if (fieldValueDict.ContainsKey("message"))
                        {
                            Received?.Invoke(this, fieldValueDict["message"]);
                        }
                        /*
                        foreach(var pair in fieldValueDict)
                        {
                            System.Diagnostics.Debug.WriteLine(pair.Key + " ::::: " + pair.Value);
                        }
                        */
                    }
                }
            }
            catch (Exception e)
            {

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
    }
}
