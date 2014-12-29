using System;

namespace Alnet.AudioServer.Components.AudioServerEndpoints
{
    internal interface IAudioServerEndpoint : IDisposable
    {
        void Start();
    }
}
