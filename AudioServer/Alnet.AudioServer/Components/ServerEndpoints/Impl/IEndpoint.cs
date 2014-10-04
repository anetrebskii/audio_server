using System;

namespace Alnet.AudioServer.Components.ServerEndpoints.Impl
{
    interface IEndpoint : IDisposable
    {
        void Start();
    }
}
