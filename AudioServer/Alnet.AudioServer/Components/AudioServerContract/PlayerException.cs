using System;
using System.Runtime.Serialization;

namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal enum PlayerExceptionTypes
    {
        NoSounds,

        NoSoundCards
    }

    [Serializable]
    internal sealed class PlayerException : Exception
    {
        /// <summary>
        /// The <see cref="Type"/> property name
        /// </summary>
        private const string TypePropertyName = "Type";

        /// <summary>
        /// The type of the exception.
        /// </summary>
        public PlayerExceptionTypes Type { get; private set; }

        public PlayerException(PlayerExceptionTypes type)
        {
            Type = type;
        }

        public PlayerException(PlayerExceptionTypes type, string message)
            : base(message)
        {
            Type = type;
        }

        public PlayerException(PlayerExceptionTypes type, string message, Exception inner)
            : base(message, inner)
        {
            Type = type;
        }

        protected PlayerException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            Type = (PlayerExceptionTypes)info.GetValue(TypePropertyName, typeof(PlayerExceptionTypes));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(TypePropertyName, Type);
            base.GetObjectData(info, context);
        }
    }
}