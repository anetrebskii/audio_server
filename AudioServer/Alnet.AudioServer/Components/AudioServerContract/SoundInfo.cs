namespace Alnet.AudioServer.Components.AudioServerContract
{
    internal sealed class SoundInfo
    {
        public SoundInfo(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { get; private set; }

        public string Url { get; private set; }
    }
}