using System;
using System.IO;
using System.Linq;
using Alnet.AudioServer.Components.AudioPlayer;

namespace Alnet.AudioServer.Components.SoundProviders
{
    class DirectorySoundProvider : ISoundProvider
    {
        private readonly string _directoryPath;

        public DirectorySoundProvider(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public SoundInfo[] GetSoundList()
        {
            return Directory.GetFiles(_directoryPath)
                .Select(filePath => new FileInfo(filePath))
                .Where(f => f.Extension.ToUpper() == ".MP3")
                .Select(f => new SoundInfo()
                {
                    Name = f.Name,
                    Url = f.FullName
                })
                .ToArray();
        }

       public event EventHandler SoundListChanged;
    }
}
