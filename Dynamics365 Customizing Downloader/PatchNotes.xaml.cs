﻿//-----------------------------------------------------------------------
// <copyright file="PatchNotes.xaml.cs" company="None">
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
    using System.Diagnostics;
    using System.Windows;
    using Update;

    /// <summary>
    /// Interaction logic for PatchNotes
    /// </summary>
    public partial class PatchNotes : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatchNotes"/> class.
        /// </summary>
        public PatchNotes()
        {
            UpdateChecker updateChecker = new UpdateChecker();
            Release release = updateChecker.GetReleaseInfo();

            this.InitializeComponent();
            this.Tbx_PatchNotes.Text = release.Body;
            this.Lbl_IsPreRelease.Content = release.Prerelease.ToString();
            this.Lbl_VersionNumber.Content = release.Name;
            this.Lbl_ReleaseDate.Content = release.Published_at.ToString();
        }

        /// <summary>
        /// Close Form
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Btn_Skip_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Download the Patch
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Btn_Download_Click(object sender, RoutedEventArgs e)
        {
            Update.UpdateChecker updateChecker = new Update.UpdateChecker();
            Uri uri = updateChecker.GetUpdateURL();

            Process.Start(uri.ToString());
        }
    }
}