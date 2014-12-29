#region Copyright

// (c) 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

#region

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

#endregion

namespace Alnet.Common
{
   /// <summary>
   /// Isolated storage helper.
   /// </summary>
   public static class IsolatedStorageExtension
   {
      #region Constants

      /// <summary>
      /// m_rootDir field name.
      /// </summary>
      private const string M_ROOT_DIR_FIELD_NAME = "m_RootDir";

      #endregion

      #region Public Methods

      /// <summary>
      /// Gets the object automatic isolated storage.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="fileName">Name of the file.</param>
      /// <param name="obj">The object.</param>
      /// <returns></returns>
      public static bool TryGetObjectFromIsolatedStorage<T>(string fileName, out T obj)
      {
         using (IsolatedStorageFile isf = IsolatedStorageFile.GetMachineStoreForAssembly())
         {
            foreach (var nameExistingFile in isf.GetFileNames().Where(nameExistingFile => nameExistingFile == fileName))
            {
               using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Open, isf))
               {
                  using (StreamReader sr = new StreamReader(isfs))
                  {
                     XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                     obj = (T)xmlSerializer.Deserialize(sr);
                     return true;
                  }
               }
            }
         }
         obj = default(T);
         return false;
      }

      /// <summary>
      /// Saves the object automatic isolated storage.
      /// </summary>
      /// <param name="obj">The object.</param>
      /// <param name="fileName">Name of the file.</param>
      public static void SaveObjectToIsolatedStorage(this Object obj, string fileName)
      {
         using (IsolatedStorageFile isf = IsolatedStorageFile.GetMachineStoreForAssembly())
         {
            using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Create, isf))
            {
               using (StreamWriter sw = new StreamWriter(isfs))
               {
                  XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                  xmlSerializer.Serialize(sw, obj);
               }
            }
         }
      }

      /// <summary>
      ///    Save file to the isolated storage.
      /// </summary>
      /// <param name="fileLocation">File location.</param>
      /// <param name="isolatedStorageFile">Isolated storage file.</param>
      /// <param name="isolatedStorageDirectoryName">Isolated storege directory name.</param>
      /// <returns>File location in the isolated storage.</returns>
      public static string SaveFileToIsolatedStorage(this IsolatedStorageFile isolatedStorageFile, string fileLocation, string isolatedStorageDirectoryName)
      {
         Guard.VerifyArgumentNotNull(isolatedStorageFile, "isolatedStorageFile");
         Guard.VerifyArgumentNotNullOrWhiteSpace(isolatedStorageDirectoryName, "isolatedStorageDirectoryName");

         using (FileStream outStream = File.OpenRead(fileLocation))
         {
            string fileName = Path.GetFileName(fileLocation);

            if (fileName == null)
            {
               return fileLocation;
            }

            if (isolatedStorageFile.FileExists(fileName))
            {
               isolatedStorageFile.DeleteFile(fileName);
            }

            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, isolatedStorageFile))
            {
               outStream.CopyTo(isoStream);
               return Path.Combine(isolatedStorageDirectoryName, fileName);
            }
         }
      }

      /// <summary>
      /// Remove file from isolated storage.
      /// </summary>
      /// <param name="fileLocation">File location.</param>
      /// <param name="isolatedStorageFile">Isoalted storage file.</param>
      public static void RemoveFileFromIsolatedStorage(this IsolatedStorageFile isolatedStorageFile, string fileLocation)
      {
         Guard.VerifyArgumentNotNull(isolatedStorageFile, "isolatedStorageFile");

         string fileName = Path.GetFileName(fileLocation);
         if (fileName != null && isolatedStorageFile.FileExists(fileName))
         {
            isolatedStorageFile.DeleteFile(fileName);
         }
      }

      /// <summary>
      /// Get isolated storage directory name.
      /// </summary>
      /// <param name="isolatedStorageFile">Isoalted storage file.</param>
      /// <returns>Isolated storage file directory name.</returns>
      public static string GetIsolatedStorageDirectoryName(this IsolatedStorageFile isolatedStorageFile)
      {
         Guard.VerifyArgumentNotNull(isolatedStorageFile, "isolatedStorageFile");

         string isolatedStorageDirectoryName = string.Empty;

         FieldInfo fieldInfo = isolatedStorageFile.GetType().GetField(M_ROOT_DIR_FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance);
         if (fieldInfo != null)
         {
            isolatedStorageDirectoryName = fieldInfo.GetValue(isolatedStorageFile).ToString();
         }

         return isolatedStorageDirectoryName;
      }

      #endregion
   }      
}
