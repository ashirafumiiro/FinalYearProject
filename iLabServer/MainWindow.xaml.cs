using NationalInstruments.Net;
using NationalInstruments;
using NationalInstruments.DAQmx;
using NationalInstruments.Tdms;
using NationalInstruments.Controls;
using NationalInstruments.Controls.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace iLabServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataSocket dataSocket = new DataSocket();
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog() { Filter = "Xml Files |*.xml" };
            if (true == openDialog.ShowDialog())
            {
                string dataFromFile = File.ReadAllText(openDialog.FileName);
                MessageBox.Show(dataFromFile);
            }
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var saveDlg = new SaveFileDialog { Filter = "xml Files |*.xml" };
            // Did they click on the OK button?
            if (true == saveDlg.ShowDialog())
            {
                string testData =
                    @"<Test><Name>Miiro</Name></Test>"; 
                // Save data in the TextBox to the named file.
                File.WriteAllText(saveDlg.FileName, testData);
            }
        }

        private void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenConfigClick(object sender, RoutedEventArgs e)
        {
            var configWindow = new ConfigWindow();
            configWindow.Owner = this;
            configWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            configWindow.ShowDialog();
        }

    }
}













/*
        private void SetOpenCommandBinding()
        {
            CommandBinding openCommand = new CommandBinding(ApplicationCommands.Open);
            openCommand.CanExecute += OpenCommand_CanExecute;
            openCommand.Executed += OpenCommand_Executed;
            CommandBindings.Add(openCommand);
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        */
