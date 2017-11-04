﻿//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="None">
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
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            cbx_connection.Items.Add("New");
            try
            {
                List<xrm.CrmConnection> crmConnections = StorageExtensions.Load();

                foreach (xrm.CrmConnection crmConnection in crmConnections)
                {
                    cbx_connection.Items.Add(crmConnection.Name);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                // Ignor File Not found
            }
        }

        private void cbx_connection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbx_connection.SelectedItem.ToString() == "New")
                {
                    ConnectionManger connectionManger = new ConnectionManger();
                    connectionManger.ShowDialog();
                    ReloadConnections();
                }
            }
            catch (NullReferenceException)
            {
                // Ignor, reload will change the index and will trigger this without items
            }
        }

        private void ReloadConnections()
        {
            try
            {
                List<xrm.CrmConnection> crmConnections = StorageExtensions.Load();
                cbx_connection.Items.Clear();

                cbx_connection.Items.Add("New");

                foreach (xrm.CrmConnection crmConnection in crmConnections)
                {
                    cbx_connection.Items.Add(crmConnection.Name);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                // Ignor File Not found
            }
        }
    }
}
