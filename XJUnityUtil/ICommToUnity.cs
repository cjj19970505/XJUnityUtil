using System;
using System.Collections.Generic;
using System.Text;

namespace XJUnityUtil
{
    /// <summary>
    /// Implement on DotNet End
    /// </summary>
    public interface ICommToUnity
    {
        event EventHandler<string> Received;

        /// <summary>
        /// "/"符号保留作控制符，不要在要发送的字符串中包含这个字符
        /// </summary>
        /// <param name="value"></param>
        void SendStringMessage(string value);
    }

    /// <summary>
    /// Implement on Unity End
    /// </summary>
    public interface ICommToApplication
    {
        event EventHandler<string> Received;
        void SendStringMessage(string value);
    }
}
