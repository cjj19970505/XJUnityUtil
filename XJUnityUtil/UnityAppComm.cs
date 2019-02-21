using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace XJUnityUtil
{
    public class UnityAppCommManager
    {
        public ICommToUnity CommToUnity { get; set; }
        public ICommToApplication CommToApplication { get; set; }

        public class Message
        {
            public Guid Uuid { get; set; }
            public Guid ResponseToUuid { get; set; }
            public string ResponseMessage { get; set; }
            public bool ResponseNeeded { get; set; }
            public bool Responsed { get; set; }
            public bool IsResponse { get; set; }
            public List<string> ValueList { get; set; }

            public Message(bool responseNeeded, params string[] values):this()
            {
                ResponseNeeded = responseNeeded;
                ValueList.AddRange(values);
            }
            public Message()
            {
                Uuid = Guid.NewGuid();
                ValueList = new List<string>();
                ResponseNeeded = false;
                Responsed = false;
                ResponseMessage = "";
                IsResponse = false;
                ResponseToUuid = Guid.Empty;
            }

            public override string ToString()
            {

                StringBuilder sb = new StringBuilder();
                sb.Append("UUID:" + Uuid);
                sb.AppendLine();
                sb.Append("ResponseNeeded:" + ResponseNeeded);
                sb.AppendLine();
                for (int i = 0; i < ValueList.Count; i++)
                {
                    sb.Append("Value_" + i + ":" + ValueList[i]);
                }

                return sb.ToString();
            }

            public string EncodeToString()
            {
                return JsonConvert.SerializeObject(this);
                
            }

            public static Message FromString(string encodedMessageString)
            {
                Message m = JsonConvert.DeserializeObject<Message>(encodedMessageString);
                return m;

            }
        }


    }


}
