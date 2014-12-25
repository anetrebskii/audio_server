using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using ApiCore;

namespace ApiCore
{
    /// <summary>
    /// Class for building url queries to vkontakte api
    /// </summary>
    public class ApiQueryBuilder
    {
        private Dictionary<string, string> paramData;

        /// <summary>
        /// Initializes api query builder
        /// </summary>
        public ApiQueryBuilder()
        {
            this.paramData = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds parameters to API request
        /// </summary>
        /// <param name="key">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Return this</returns>
        public ApiQueryBuilder Add(string key, string value)
        {
            this.paramData.Add(key, value);
            return this;
        }

        /// <summary>
        /// Clear api parameters
        /// </summary>
        public void Clear()
        {
            this.paramData.Clear();
        }

        /// <summary>
        /// Build query string
        /// </summary>
        /// <returns>Ready query string</returns>
        public string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            string methodName = this.paramData["method"];
            sb.AppendFormat("https://api.vk.com/method/{0}?", methodName);
            sb.Append(String.Join("&", paramData.Select(p => String.Format("{0}={1}", p.Key, p.Value))));
            return sb.ToString();
        }
    }
}
