using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XJUnityUtil
{
    public delegate void CommToApplicationReceiveMessageHandler(ICommToApplication CommToApp, UnityAppCommManager.Message message);
    /// <summary>
    /// Implement on DotNet End
    /// </summary>
    public interface ICommToUnity
    {
        event EventHandler<UnityAppCommManager.Message> Received;

        void SendStringMessage(params string[] values);
        Task<string> SendStringMessageForResultAsync(params string[] values);

    }

    /// <summary>
    /// Implement on Unity End
    /// </summary>
    public interface ICommToApplication
    {
        event CommToApplicationReceiveMessageHandler Received;
        void SendStringMessage(params string[] values);
        void SendResult(UnityAppCommManager.Message message, string responseValue);
    }
}
