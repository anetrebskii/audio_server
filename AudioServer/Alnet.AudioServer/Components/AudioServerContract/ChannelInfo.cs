using System;

namespace Alnet.AudioServer.Components.AudioServerContract
{
   /// <summary>
   /// Represent the audio out 
   /// </summary>
    internal sealed class ChannelInfo
    {
       /// <summary>
       /// Initializes a new instance of the <see cref="ChannelInfo"/> class.
       /// </summary>
       /// <param name="index">The index (id).</param>
       /// <param name="nativeDesciption">The native desciption. Eg. computer audio card name.</param>
       /// <param name="description">The description for end user.</param>
        public ChannelInfo(int index, string nativeDesciption, string description)
        {
            Index = index;
            NativeDesciption = nativeDesciption;
            Desciption = description;
        }

        /// <summary>
        /// The index (id).
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The native desciption. Eg. computer audio card name.
        /// </summary>
        public string NativeDesciption { get; private set; }

        /// <summary>
        /// The description for end user.
        /// </summary>
        public string Desciption { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}-{1}-{2}", Index, NativeDesciption, Desciption);
        }
    }
}