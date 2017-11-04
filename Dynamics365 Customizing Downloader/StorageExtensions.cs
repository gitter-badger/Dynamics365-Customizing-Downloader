﻿//-----------------------------------------------------------------------
// <copyright file="StorageExtensions.cs" company="None">
// Copyright 2017 Jhueppauff
// MIT  
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

namespace Dynamics365CustomizingDownloader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Json.Net Storage Adapter for saving CRM Connection to file
    /// </summary>
    public static class StorageExtensions
    {
        /// <summary>
        /// Local Path of the JSON Storage File
        /// </summary>
        private static string storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dyn365_Configuration.json");

        /// <summary>
        /// Saves the <see cref="xrm.CrmConnection"/> to the local Configuration
        /// </summary>
        /// <param name="crmConnection">CRM Connection <see cref="xrm.CrmConnection"/></param>
        public static void Save(xrm.CrmConnection crmConnection)
        {
            List<xrm.CrmConnection> crmConnections = new List<xrm.CrmConnection>();

            // Create if File does not exist
            if (!File.Exists(storagePath))
            {
                File.Create(storagePath).Close();

                // Encrypt Connection String
                crmConnection.ConnectionString = Cryptography.EncryptStringAES(crmConnection.ConnectionString, "SharedSecret_547ä#Dyn354");
                crmConnections.Add(crmConnection);
                string json = JsonConvert.SerializeObject(crmConnections);

                File.WriteAllText(storagePath, json);
            }
            else
            {
                using (FileStream fs = File.Open(storagePath, FileMode.Open))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    byte[] b = new byte[1024];
                    UTF8Encoding temp = new UTF8Encoding(true);

                    while (fs.Read(b, 0, b.Length) > 0)
                    {
                        stringBuilder.AppendLine(temp.GetString(b));
                    }

                    // Convert Json to List
                    crmConnections = JsonConvert.DeserializeObject<List<xrm.CrmConnection>>(stringBuilder.ToString());

                    // Encrypt Connection String
                    crmConnection.ConnectionString = Cryptography.EncryptStringAES(crmConnection.ConnectionString, "SharedSecret_547ä#Dyn354");
                    crmConnections.Add(crmConnection);

                    // Close File Stream
                    fs.Flush();
                    fs.Close();
                }

                // Write to Configuration File
                string json = JsonConvert.SerializeObject(crmConnections);
                File.WriteAllText(storagePath, json);
            }
        }

        /// <summary>
        /// Loads the current configuration
        /// </summary>
        /// <returns>Returns <see cref="List{xrm.CrmConnection}"/></returns>
        public static List<xrm.CrmConnection> Load()
        {
            List<xrm.CrmConnection> crmConnections = new List<xrm.CrmConnection>();

            if (File.Exists(storagePath))
            {
                using (FileStream fs = File.OpenRead(storagePath))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    byte[] b = new byte[1024];
                    UTF8Encoding temp = new UTF8Encoding(true);

                    while (fs.Read(b, 0, b.Length) > 0)
                    {
                        stringBuilder.AppendLine(temp.GetString(b));
                    }

                    // Converts Json to List
                    crmConnections = JsonConvert.DeserializeObject<List<xrm.CrmConnection>>(stringBuilder.ToString());

                    foreach (xrm.CrmConnection crmTempConnection in crmConnections)
                    {
                        crmTempConnection.ConnectionString = Cryptography.DecryptStringAES(crmTempConnection.ConnectionString, "SharedSecret_547ä#Dyn354");
                    }

                    // Close File Stream
                    fs.Flush();
                    fs.Close();
                }
            }
            else
            {
                throw new FileNotFoundException("File was not found");
            }

            return crmConnections;
        }
    }
}