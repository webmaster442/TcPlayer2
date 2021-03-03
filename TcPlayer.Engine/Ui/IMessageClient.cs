using System;

namespace TcPlayer.Engine.Ui
{
    public interface IMessageClient
    {
        /// <summary>
        /// Message reciever ID;
        /// </summary>
        Guid MessageReciverID { get; }
    }

    public interface IMessageClient<in Tmsg> : IMessageClient
    {
        /// <summary>
        /// Handler for a message
        /// </summary>
        /// <param name="message">Message to handle</param>
        void HandleMessage(Tmsg message);
    }
}
