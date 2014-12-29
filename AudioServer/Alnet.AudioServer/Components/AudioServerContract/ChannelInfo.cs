using System;

namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal sealed class ChannelInfo
    {
        public ChannelInfo(int index, string description)
        {
            Index = index;
            Description = description;
        }

        public int Index { get; private set; }

        public string Description { get; private set; }

        public override string ToString()
        {
            return String.Format("{0}-{1}", Index, Description);
        }
    }
}