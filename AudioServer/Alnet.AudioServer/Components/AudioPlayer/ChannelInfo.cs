using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alnet.AudioServer.Components.AudioPlayer
{
   internal sealed class ChannelInfo
   {
      public int Index { get; set; }
      public string Description { get; set; }

      public override string ToString()
      {
         return String.Format("{0}-{1}", Index, Description);
      }
   }
}
