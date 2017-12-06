﻿//-----------------------------------------------------------------------
// <copyright file="CrmConnection.cs" company="None">
// Copyright 2017 Jhueppauff
// MIT  
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

namespace Dynamics365CustomizingDownloader.Xrm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// CRM Connection
    /// </summary>
    public class CrmConnection
    {
        /// <summary>
        /// Gets or sets the ID of the CRM Connection
        /// </summary>
        public Guid ConnectionID { get; set; }

        /// <summary>
        /// Gets or sets the name of the CRM Connection = Organization Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Connection String
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the local path
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or sets the CRM Solutions for this connection
        /// </summary>
        public List<CrmSolution> CRMSolutions { get; set; }
    }
}
