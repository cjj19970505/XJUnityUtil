using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace XJUnityUtil.Unity
{
    public class EditorCommToUnity : MonoBehaviour, ICommToApplication
    {
        public int Port;
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
                UnityWebRequest www = UnityWebRequest.Get("http://localhost:" + Port + "/getmessage/");
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
    }
}


