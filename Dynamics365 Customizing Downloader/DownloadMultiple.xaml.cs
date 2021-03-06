﻿//-----------------------------------------------------------------------
// <copyright file="DownloadMultiple.xaml.cs" company="https://github.com/jhueppauff/Dynamics365-Customizing-Downloader">
// Copyright 2018 Jhueppauff
// Mozilla Public License Version 2.0 
// For licence details visit https://github.com/jhueppauff/Dynamics365-Customizing-Downloader/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace Dynamics365CustomizingDownloader
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Dynamics365CustomizingDownloader.Core.Xrm;
    using Microsoft.Xrm.Tooling.Connector;

    /// <summary>
    /// Interaction logic for DownloadMultiple
    /// </summary>
    public partial class DownloadMultiple : Window
    {
        /// <summary>
        /// Log4Net Logger
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Static Status Text Box, used for multi threading UI Update
        /// </summary>
        private static TextBox statusTextBox = new TextBox();

        /// <summary>
        /// BackGround Worker
        /// </summary>
        private readonly BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// CRM Service Client
        /// </summary>
        private CrmServiceClient crmServiceClient = null;

        /// <summary>
        /// Dispose bool
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Indicates if the panel is loading
        /// </summary>
        private bool panelLoading;

        /// <summary>
        /// Panel Message
        /// </summary>
        private string panelMainMessage = "Please wait, downloading and extracting Solution";

        /// <summary>
        /// Selected local Path
        /// </summary>
        private string selectedPath;

        /// <summary>
        /// Specifies if Solution Strings should be exported into Resource File
        /// </summary>
        private bool localizeSupport;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadMultiple"/> class.
        /// </summary>
        /// <param name="crmConnection"><see cref="Xrm.CrmConnection"/> for the XRM Connector</param>
        /// <param name="crmSolutions"><see cref="List{Xrm.CrmSolution}"/> of all Solutions to Download</param>
        public DownloadMultiple(Core.Xrm.CrmConnection crmConnection, List<Core.Xrm.CrmSolution> crmSolutions)
        {
            this.InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            DownloadMultiple.statusTextBox = this.tbx_status;
            DownloadMultiple.LogToUI("Started Form");
            this.Btn_close.IsEnabled = false;
            this.CRMConnection = crmConnection;
            DownloadMultiple.LogToUI($"CRM Connection: {this.CRMConnection.Name}", true);
            this.CRMSolutions = crmSolutions;

            foreach (Core.Xrm.CrmSolution crmSolution in this.CRMSolutions)
            {
                DownloadMultiple.LogToUI($"Added Solution: { crmSolution.UniqueName} to Download List", true);
            }

            this.tbx_download.Text = crmConnection.LocalPath;
            DownloadMultiple.LogToUI($"Pulled Path form config: {crmConnection.LocalPath}", true);

            this.Owner = App.Current.MainWindow;
        }

        /// <summary>
        /// Delegated UI Update 
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="isDebugging">Identifies if the message is a Debug Message.</param>
        public delegate void UpdateTextCallback(string message, bool isDebugging);

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a <see cref="List{Xrm.CrmSolution}"/> of all Solutions to Download
        /// </summary>
        public List<Core.Xrm.CrmSolution> CRMSolutions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Xrm.CrmConnection"/> for the XRM Connector
        /// </summary>
        public Core.Xrm.CrmConnection CRMConnection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [panel loading].
        /// </summary>
        /// <value>
        /// <c>true</c> if [panel loading]; otherwise, <c>false</c>.
        /// </value>
        public bool PanelLoading
        {
            get
            {
                return this.panelLoading;
            }

            set
            {
                this.panelLoading = value;
                this.RaisePropertyChanged("PanelLoading");
            }
        }

        /// <summary>
        /// Gets or sets the panel main message.
        /// </summary>
        /// <value>The panel main message.</value>
        public string PanelMainMessage
        {
            get
            {
                return this.panelMainMessage;
            }

            set
            {
                this.panelMainMessage = value;
                this.RaisePropertyChanged("PanelMainMessage");
            }
        }

        /// <summary>
        /// Gets the hide panel command.
        /// </summary>
        public ICommand HidePanelCommand => new HelperClasses.DelegateCommand(() =>
        {
            PanelLoading = false;
        });

        /// <summary>
        /// Updated the UI Status Text Box
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="isDebugging">Identifies if the message is a Debugging Message, Default = false</param>
        public static void LogToUI(string message, bool isDebugging = false)
        {
            if (isDebugging && (bool)App.Current.Properties["Debugging.Enabled"])
            {
                // Debug Messages
                statusTextBox.Text += $"{DateTime.Now} : Debug: {message}\n";
                statusTextBox.CaretIndex = statusTextBox.Text.Length;
                statusTextBox.ScrollToEnd();
            }
            else if (!isDebugging)
            {
                statusTextBox.Text += $"{DateTime.Now} : {message}\n";
                statusTextBox.CaretIndex = statusTextBox.Text.Length;
                statusTextBox.ScrollToEnd();
            }
        }

        /// <summary>
        /// Updates the UI delegated
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="isDebugging">Identifies if the message is a Debugging Message</param>
        public static void UpdateUI(string message, bool isDebugging)
        {
            statusTextBox.Dispatcher.Invoke(
                new UpdateTextCallback(LogToUI),
                message,
                isDebugging);
        }

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose class
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    this.worker.Dispose();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                this.disposed = true;
            }
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Closes the Form on Button Action
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Button Action, Download
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.tbx_download.Text != string.Empty)
            {
                this.loadingPanel.IsLoading = true;
                this.selectedPath = tbx_download.Text;

                Core.Xrm.CrmConnection crmConnection = new Core.Xrm.CrmConnection
                {
                    ConnectionID = Core.Data.StorageExtensions.FindConnectionIDByName(this.CRMConnection.Name),
                    ConnectionString = this.CRMConnection.ConnectionString,
                    LocalPath = this.selectedPath,
                    Name = this.CRMConnection.Name
                };
                this.CRMConnection = crmConnection;

                if ((bool)Cbx_ExportLables.IsChecked)
                {
                    this.localizeSupport = true;
                }
                else
                {
                    this.localizeSupport = false;
                }

                // Update Connection
                Core.Data.StorageExtensions.Update(crmConnection, MainWindow.EncryptionKey);

                // Background Worker
                this.worker.DoWork += this.Worker_DoWork;
                this.worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
                this.worker.WorkerReportsProgress = true;
                this.worker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Path can not be empty", "Path empty", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Background Worker Event DoWork
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Create Folder if it does not exists
                if (!Directory.Exists(this.CRMConnection.LocalPath))
                {
                    Directory.CreateDirectory(this.CRMConnection.LocalPath);
                }

                foreach (CrmSolution solution in this.CRMSolutions)
                {
                    if (!this.worker.CancellationPending)
                    {
                        this.ProcessSolutions(solution);
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                DownloadMultiple.UpdateUI($"An Error occured: {ex.Message}", false);
                DownloadMultiple.Log.Error(ex.Message, ex);

                // Open Error Report 
                if (!Properties.Settings.Default.DisableErrorReports)
                {
                    throw;
                }
            }
            finally
            {
                if (this.crmServiceClient != null)
                {
                    this.crmServiceClient.Dispose();
                }
            }
        }

        /// <summary>
        /// Will Extract the Solution into the folder
        /// </summary>
        /// <param name="solution">CRM Solution which should be extracted</param>
        private void ProcessSolutions(CrmSolution solution)
        {
            using (ToolingConnector toolingConnector = new ToolingConnector())
            {
                // Delete Solution File if it exists
                if (File.Exists(Path.Combine(this.selectedPath, solution.UniqueName + ".zip")))
                {
                    File.Delete(Path.Combine(this.selectedPath, solution.UniqueName + ".zip"));
                }

                this.crmServiceClient = toolingConnector.GetCrmServiceClient(connectionString: this.CRMConnection.ConnectionString);

                try
                {
                    toolingConnector.DownloadSolution(this.crmServiceClient, solution.UniqueName, this.selectedPath);

                    CrmSolutionPackager crmSolutionPackager = new CrmSolutionPackager();

                    if (Directory.Exists(Path.Combine(this.selectedPath, solution.UniqueName)))
                    {
                        Core.IO.Directory.DeleteDirectory(Path.Combine(this.selectedPath, solution.UniqueName));
                        DownloadMultiple.UpdateUI($"Delete {Path.Combine(this.selectedPath, solution.UniqueName).ToString()}", true);
                    }

                    string log = crmSolutionPackager.ExtractCustomizing(Path.Combine(this.selectedPath, solution.UniqueName + ".zip"), Path.Combine(this.selectedPath, solution.UniqueName), Properties.Settings.Default.SolutionPackagerLogPath, this.localizeSupport);
                    DownloadMultiple.UpdateUI(log, false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                    if (!Properties.Settings.Default.DisableErrorReports)
                    {
                        throw;
                    }
                }
                finally
                {
                    if (File.Exists(Path.Combine(this.selectedPath, solution.UniqueName + ".zip").ToString()))
                    {
                        File.Delete(Path.Combine(this.selectedPath, solution.UniqueName + ".zip"));
                        DownloadMultiple.UpdateUI($"Delete {Path.Combine(this.selectedPath, solution.UniqueName + ".zip").ToString()}", true);
                    }
                }
            }
        }

        /// <summary>
        /// Background Worker Event Worker_RunWorkerCompleted
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Diagnostics.ErrorReport errorReport = new Diagnostics.ErrorReport(e.Error, "An error occurred while downloading or extracting the solution");
                errorReport.Show();
            }

            this.loadingPanel.IsLoading = false;
            this.Btn_close.IsEnabled = true;
            DownloadMultiple.LogToUI("---------------");
            DownloadMultiple.LogToUI("Finished download/extraction");
            DownloadMultiple.LogToUI("---------------");
        }

        /// <summary>
        /// Form Closing Event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        private void DownloadWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.worker.IsBusy)
            {
                MessageBoxResult dialogResult = MessageBox.Show("Download is still running, are you sure to abort the process?", "Background thread is still active!", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (dialogResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        this.worker.CancelAsync();
                    }
                    catch (Exception)
                    {
                        // ignore an issue, as the worker should be terminated on dialog close as well.
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Opens the select Folder dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Btn_SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    tbx_download.Text = dialog.SelectedPath.ToString();
                }
            }
        }
    }
}