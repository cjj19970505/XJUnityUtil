﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace XJUnityUtil.Unity
{
    public class EditorCommToApp : MonoBehaviour, ICommToApplication
    {
        public int EditorDebugPort;
        public event EventHandler<string> Received;
        private List<string> _ReceivedMessageBuffer;

        // Start is called before the first frame update
        void Start()
        {
            _ReceivedMessageBuffer = new List<string>();
            StartCoroutine(_GetText());
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var message in _ReceivedMessageBuffer)
            {
                Received?.Invoke(this, message);
            }
            _ReceivedMessageBuffer.Clear();
        }

        private IEnumerator _GetText()
        {
            while (true)
            {
                UnityWebRequest www = UnityWebRequest.Get("http://localhost:" + EditorDebugPort + "/getmessage/");
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string[] messages = www.downloadHandler.text.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string message in messages)
                    {
                        _ReceivedMessageBuffer.Add(message);
                        Debug.Log("NEW_MESSAGE::" + message);
                    }
                }
            }
        }

        public void SendStringMessage(string value)
        {
            StartCoroutine(_EditorSendDataPostRequest(value));
        }

        IEnumerator _EditorSendDataPostRequest(string message)
        {
            WWWForm form = new WWWForm();
            form.AddField("MESSAGE", message);
            WWW www = new WWW("http://localhost:" + EditorDebugPort + "/", form);
            yield return www;
        }
    }
}


