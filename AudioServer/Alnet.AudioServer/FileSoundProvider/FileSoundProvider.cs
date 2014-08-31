using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alnet.AudioServer.AudioPlayer;

namespace Alnet.AudioServer.FileSoundProvider
{
    class FileSoundProvider : ISoundProvider
    {
        private readonly string _directoryPath;
        private SoundInfo[] _sounds;

        public FileSoundProvider(string directoryPath)
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
