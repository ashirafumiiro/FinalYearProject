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
using System.Data;

namespace iLabServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataSocket dataSocket = new DataSocket();
        string configDirectory = System.IO.Path
            .Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LabServer");
        string currentFile = "";
        
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
                //ToDo 
                //Verify whether it is a valid lab file


                GetXmlData(openDialog.FileName);
            }
        }

        private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveChanges();
            //var saveDlg = new SaveFileDialog { Filter = "xml Files |*.xml" };
            //// Did they click on the OK button?
            //if (true == saveDlg.ShowDialog())
            //{
            //    string testData =
            //        @"<Test><Name>Miiro</Name></Test>"; 
            //    // Save data in the TextBox to the named file.
            //    File.WriteAllText(saveDlg.FileName, testData);
            //}
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

        private void GetXmlData(string path)
        {                               
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(path);
            DataView dataView = new DataView(dataSet.Tables[0]);
            xmlDataGrid.ItemsSource = dataView;
        }

        private void FileUpdate_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            DataTable dt = new DataTable();
            dt = ((DataView)xmlDataGrid.ItemsSource).ToTable();
            DataSet data = new DataSet("Settings");
            data.Tables.Add(dt);
            data.WriteXml(@"C:\Users\ashir\Documents\LabServer\Labs\halfwave.xml");
            MessageBox.Show("Done");
        }

    }
}