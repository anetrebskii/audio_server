#region Copyright

// (c) 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.Data.SqlClient;
using CCTV.Framework.Utility;

namespace Alnet.Common
{
   /// <summary>
   ///    Utility class that provide help methods for database working.
   /// </summary>
   public static class DataBaseUtils
   {
      #region Constants

      /// <summary>
      ///    Divider character between server name and database name in connection string.
      /// </summary>
      private const string DEVIDER_BETWEEN_SERVER_NAME_AND_DATABASE_NAME = "\\";

      /// <summary>
      ///    Divider character between server source and other information in connection string.
      /// </summary>
      private const string DEVIDER_BETWEEN_DATASOURCE_AND_OTHER_INFORMATION = ",";

      /// <summary>
      ///    Server name for local machine.
      /// </summary>
      private const string LOCALHOST_SERVER_NAME = "localhost";

      /// <summary>
      ///    Local machine another names in connection string.
      /// </summary>
      private static readonly string[] LocalMachineSynonims = new[] {".", "(local)"};

      #endregion

      #region Public Methods

      /// <summary>
      ///    Get database server name from connection string.
      /// </summary>
      /// <param name="connectionString">Connection to Db for parsing.</param>
      /// <returns>Server name.</returns>
      public static string GetDataBaseServerName(this string connectionString)
      {
         Guard.VerifyArgumentNotNullOrEmpty (connectionString, "connectionString");

         string dataSource = null;
         using (SqlConnection connection = new SqlConnection (connectionString))
         {
            dataSource = connection.DataSource;
         }

         if (!string.IsNullOrEmpty (dataSource))
         {
            string[] splittedDataSource = dataSource.Split (new[] {DEVIDER_BETWEEN_DATASOURCE_AND_OTHER_INFORMATION}, StringSplitOptions.RemoveEmptyEntries);
            if (splittedDataSource.Length > 0)
            {
               dataSource = splittedDataSource[0];
            }

            string serverName = dataSource;
            string[] splittedServerName = serverName.Split (new[] {DEVIDER_BETWEEN_SERVER_NAME_AND_DATABASE_NAME}, StringSplitOptions.RemoveEmptyEntries);
            if (splittedServerName.Length > 0)
            {
               serverName = splittedServerName[0];
            }

            if (Array.Exists (LocalMachineSynonims, s => string.Equals (s, serverName, StringComparison.InvariantCultureIgnoreCase)))
            {
               return LOCALHOST_SERVER_NAME;
            }

            return serverName;
         }

         return LOCALHOST_SERVER_NAME;
      }

      /// <summary>
      /// Indicates whether specified value is <see langword="null"/> or <see cref="DBNull"/>.
      /// </summary>
      /// <param name="value">Value to test.</param>
      /// <returns><see langword="true"/> if the value parameter is null or DBNull otherwise, false. </returns>
      public static bool IsNullOrDBNull(object value)
      {
         return value == null || Convert.IsDBNull(value);
      }

      #endregion
   }
}