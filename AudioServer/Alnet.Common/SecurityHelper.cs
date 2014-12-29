#region Copyright

// © 2012 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CCTV.Framework.Utility
{
   /// <summary>
   ///   Some helper methods for security and access control.
   /// </summary>
   public static class SecurityHelper
   {
      #region Public Methods

      /// <summary>
      ///   Checks that application have write permission to specified directory.
      /// </summary>
      /// <param name="path"> Directory to verify write access. </param>
      /// <returns> <see langword="true" /> if write access granted. </returns>
      public static bool HasWritePermissionOnDir(string path)
      {
         var writeAllow = false;
         var writeDeny = false;
         var accessControlList = Directory.GetAccessControl(path);
         if (accessControlList == null)
         {
            return false;
         }
         var accessRules = accessControlList.GetAccessRules(true, true, typeof(SecurityIdentifier));
         if (accessRules == null)
         {
            return false;
         }

         foreach (FileSystemAccessRule rule in accessRules)
         {
            if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
            {
               continue;
            }

            if (rule.AccessControlType == AccessControlType.Allow)
            {
               writeAllow = true;
            }
            else if (rule.AccessControlType == AccessControlType.Deny)
            {
               writeDeny = true;
            }
         }

         return writeAllow && !writeDeny;
      }

      #endregion
   }
}