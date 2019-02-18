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
