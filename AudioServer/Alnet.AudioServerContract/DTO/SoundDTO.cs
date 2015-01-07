using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Alnet.AudioServerContract.DTO
{
    [DataContract]
    public sealed class SoundDTO
    {
        [DataMember]
        public string Name { get; set; }
    }
}
