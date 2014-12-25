using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Alnet.AudioServer.Components.AudioPlayer
{
   [Serializable]
   public class PlayerException : Exception
   {
      //
      // For guidelines regarding the creation of new exception types, see
      //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
      // and
      //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
      //

      public PlayerException()
      {
      }

      public PlayerException(string message) : base(message)
      {
      }

      public PlayerException(string message, Exception inner) : base(message, inner)
      {
      }

      protected PlayerException(
         SerializationInfo info,
         StreamingContext context) : base(info, context)
      {
      }
   }
}
