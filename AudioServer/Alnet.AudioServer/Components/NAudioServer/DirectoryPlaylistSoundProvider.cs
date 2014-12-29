using System;
using System.IO;
using System.Linq;
using Alnet.AudioServer.Components.AudioServerContract;
using Alnet.Common;

namespace Alnet.AudioServer.Components.NAudioServer
{
    internal sealed class DirectoryPlaylistSoundProvider : IPlaylistSoundProvider
    {
        #region Private fields

        /// <summary>
        /// Directory for load audio files.
        /// </summary>
        private readonly string _directoryPath;

        #endregion

        #region Constructors

        public DirectoryPlaylistSoundProvider(string directoryPath)
        {
            _directoryPath = Guard.EnsureArgumentNotNullOrWhiteSpace(directoryPath, "directoryPath");
        }

        #endregion

        #region IPlaylistSoundProvider

        public SoundInfo[] GetSounds()
        {
            return Directory.GetFiles(_directoryPath)
                .Select(filePath => new FileInfo(filePath))
                .Where(f => f.Extension.ToUpper() == ".MP3")
                .Select(f => new SoundInfo(f.Name, f.FullName))
                .ToArray();
        }

        public event EventHandler SoundsChanged;

        #endregion
    }
}
