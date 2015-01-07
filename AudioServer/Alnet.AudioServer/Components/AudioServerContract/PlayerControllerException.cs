using System;
using System.Runtime.Serialization;

namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal enum PlayerControllerExceptionTypes
    {
        NoPlayer,
    }

    [Serializable]
    internal sealed class PlayerControllerException : Exception
    {
        /// <summary>
        /// The <see cref="Type"/> property name
        /// </summary>
        private const string TypePropertyName = "Type";

        /// <summary>
        /// The type of the exception.
        /// </summary>
        public PlayerControllerExceptionTypes Type { get; private set; }

        public PlayerControllerException(PlayerControllerExceptionTypes type)
        {
            Type = type;
        }

        public PlayerControllerException(PlayerControllerExceptionTypes type, string message)
            : base(message)
        {
            Type = type;
        }

        public PlayerControllerException(PlayerControllerExceptionTypes type, string message, Exception inner)
            : base(message, inner)
        {
            Type = type;
        }

        protected PlayerControllerException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            Type = (PlayerControllerExceptionTypes)info.GetValue(TypePropertyName, typeof(PlayerExceptionTypes));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(TypePropertyName, Type);
            base.GetObjectData(info, context);
        }
    }
}