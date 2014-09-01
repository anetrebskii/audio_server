using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alnet.AudioServer.Endpoints
{
    interface IEndpoint : IDisposable
    {
        void Start();
    }
}
