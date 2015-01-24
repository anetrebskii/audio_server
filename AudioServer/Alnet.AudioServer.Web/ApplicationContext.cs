using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Alnet.AudioServer.Web.AudioServerService;
using Alnet.AudioServer.Web.Components.AudioServerService;

namespace Alnet.AudioServer.Web
{
    internal class ApplicationContext
    {
        #region Singleton

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static readonly ApplicationContext _instance = new ApplicationContext();

        /// <summary>
        /// Prevents a default instance of the <see cref="ApplicationContext"/> class from being created.
        /// </summary>
        private ApplicationContext()
        {
            AudioPlayerService = new AudioServerServiceProxy();
        }

        /// <summary>
        /// The singleton instance
        /// </summary>
        public static ApplicationContext Instance
        {
            get { return _instance; }
        } 

        #endregion

        public IAudioServerService AudioPlayerService { get; private set; }
    }
}