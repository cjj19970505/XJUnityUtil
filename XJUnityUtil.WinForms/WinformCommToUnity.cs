using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XJUnityUtil;

namespace XJUnityUtil.WinForms
{
    public class WinformCommToUnity : ICommToUnity
    {
        public event EventHandler<string> Received;

        public ChromiumWebBrowser CefBrowser { get; }

        //public object _Waited
        public Dictionary<Guid, UnityAppCommManager.Message> _WaitForResponseMessages;
        public object _WaitForResponseMessagesLock;
        /// <summary>
        /// key是要回复的Message的Guid
        /// </summary>
        public Dictionary<Guid, UnityAppCommManager.Message> _ResponseMessages;
        private object _ResponseMessagesLock;
        public WinformCommToUnity(ChromiumWebBrowser cefBrowser)
        {
            CefBrowser = cefBrowser;
            
            _WaitForResponseMessages = new Dictionary<Guid, UnityAppCommManager.Message>();
            _WaitForResponseMessagesLock = new object();
            _ResponseMessages = new Dictionary<Guid, UnityAppCommManager.Message>();
            _ResponseMessagesLock = new object();

            
        }

        public void SendStringMessage(params string[] values)
        {
            UnityAppCommManager.Message message = new UnityAppCommManager.Message(false, values);
            CefBrowser.GetMainFrame().ExecuteJavaScriptAsync("gameInstance.SendMessage(\"XJUnityUtilCenter\", \"OnMessageReceived\", \"" + HttpUtility.UrlEncode(message.EncodeToString()) + "\")");
        }

        public Task<string> SendStringMessageForResultAsync(params string[] values)
        {
            UnityAppCommManager.Message message = new UnityAppCommManager.Message();
            message.ResponseNeeded = true;
            message.ValueList.AddRange(values);
            lock (_WaitForResponseMessagesLock)
            {
                _WaitForResponseMessages.Add(message.Uuid, message);
            }
            CefBrowser.GetMainFrame().ExecuteJavaScriptAsync("gameInstance.SendMessage(\"XJUnityUtilCenter\", \"OnMessageReceived\", \"" + HttpUtility.UrlEncode(message.EncodeToString()) + "\")");
            return Task.Run<string>(() =>
            {
                while (true)
                {
                    lock (_ResponseMessagesLock)
                    {
                        if (_ResponseMessages.ContainsKey(message.Uuid))
                        {
                            var responseMessage = _ResponseMessages[message.Uuid];
                            _ResponseMessages.Remove(message.Uuid);
                            lock (_WaitForResponseMessagesLock)
                            {
                                _WaitForResponseMessages.Remove(message.Uuid);
                            }
                            return responseMessage.ValueList[0];
                        }
                    }
                    
                }
            });

        }
        private string _EncodeValues(params string[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var value in values)
            {
                sb.Append(HttpUtility.UrlEncode(value));
                sb.Append("/");
            }
            return sb.ToString();
        }

        public class CefUnityHandlerObject
        {
            public WinformCommToUnity WinformCommToUnity { get; }
            public CefUnityHandlerObject(WinformCommToUnity winformCommToUnity)
            {
                WinformCommToUnity = winformCommToUnity;
            }

            public void JsToApp(string s)
            {
                var message = UnityAppCommManager.Message.FromString(s);
                if (message.IsResponse)
                {
                    lock (WinformCommToUnity._ResponseMessagesLock)
                    {
                        WinformCommToUnity._ResponseMessages.Add(message.ResponseToUuid, message);
                    }
                }
                else
                {
                    WinformCommToUnity.Received?.Invoke(WinformCommToUnity, s);
                }
                
            }
        }
    }

    
}
