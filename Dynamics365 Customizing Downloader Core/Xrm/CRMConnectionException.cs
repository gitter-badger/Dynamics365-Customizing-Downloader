﻿//-----------------------------------------------------------------------
// <copyright file="CRMConnectionException.cs" company="https://github.com/jhueppauff/Dynamics365-Customizing-Downloader">
// Copyright 2017 Jhueppauff
// MIT  
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

namespace Dynamics365CustomizingDownloader.Core.Xrm
{
    using System;

    /// <summary>
    /// CRM Connection Exception
    /// </summary>
    public class CRMConnectionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CRMConnectionException"/> class.
        /// </summary>
        public CRMConnectionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CRMConnectionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">Exception Message</param>
        public CRMConnectionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CRMConnectionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">Exception Message</param>
        /// <param name="inner">Inner Exception Details</param>
        public CRMConnectionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}