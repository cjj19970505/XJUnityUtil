using System;
using System.Collections.Generic;
using System.Text;

namespace XJUnityUtil
{
    public class UnityAppCommManager
    {
        public ICommToUnity CommToUnity { get; set; }
        public ICommToApplication CommToApplication { get; set; }

        public class Message
        {
            public Guid Uuid { get; set; }
            //public string Value { get; set; }
            public List<string> ValueList { get; }
            public bool ResponseNeeded { get; set; }
            public bool Responsed { get; set; }
            public string ResponseMessage { get; set; }
            public Message(bool responseNeeded, params string[] values)
            {
                ValueList = new List<string>();
                Uuid = Guid.NewGuid();
                ResponseNeeded = responseNeeded;
                Responsed = false;
                ValueList.AddRange(values);
            }
            public Message()
            {
                ValueList = new List<string>();
                Responsed = false;
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
        }


    }


}
