using System.IO;
using System.Linq;
using Alnet.AudioServer.Components.AudioPlayer;

namespace Alnet.AudioServer.Components.SoundProviders
{
    class DirectorySoundProvider : ISoundProvider
    {
        private readonly string _directoryPath;
        private SoundInfo[] _sounds;

        public DirectorySoundProvider(string directoryPath)
        {
            _directoryPath = directoryPath;
            _sounds = Directory.GetFiles(_directoryPath)
                .Select(filePath => new FileInfo(filePath))
                .Where(f => f.Extension.ToUpper() == ".MP3")
                .Select(f => new SoundInfo()
                {
                    Name = f.Name,
                    Url = f.FullName
                })
                .ToArray();
        }

        public SoundInfo[] GetSoundList()
        {
            return _sounds;
        }

        public byte[] GetSoundData(int index)
        {
            return File.ReadAllBytes(_sounds[index].Url);
        }
    }
}
