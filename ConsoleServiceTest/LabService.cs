using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using NationalInstruments.Net;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace LabServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class LabService 
    {
        private int numOfErrors = 0;
        int channels = 2;
        System.Timers.Timer timer;

        string workingPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "iLabServer");

        public LabService()
        {          
            StartWebServer();
            // enitialise time
            timer = new System.Timers.Timer();
            timer.Interval = 500; // in milli seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            
        }

        public void CreateComponents()
        {
            InitializeComponent();
            CreateChannels(channels);
            CreateScopeTask("Dev1", channels);
        }
     
        
        public void Start()
        {
            CreateComponents();
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;     
            DisposeLab();
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                NationalInstruments.AnalogWaveform<double>[] acquiredData = scopeTaskComponent.Read();

                int i = 0;
                foreach (var dataSocket in this.components.Components.OfType<DataSocket>())
                {
                    if (!dataSocket.IsConnected)
                    {
                        ConnectDataSockets();
                    }

                    if (dataSocket.IsConnected)
                    {
                        dataSocket.Data.Value = acquiredData[i].GetRawData(); ;
                    }
                    else
                    {
                        ConnectDataSockets();
                    }
                    i++;
                }
                numOfErrors = 0;
            }
            catch (NationalInstruments.DAQmx.DaqException ex)
            {
                if (numOfErrors > 5) // Fail fivetimes
                {
                    timer.Enabled = false;
                    DialogResult dialogResult = MessageBox.Show(ex.Message +
                        "\nTry Again?",
                        "Daq System Error", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        timer.Enabled = true;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        timer.Enabled = false;
                    }

                    // Save error to file in workingFolder
                    //ToDo
                }
                
                numOfErrors++;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                numOfErrors++;
            }
        }

        public void ConnectDataSockets()
        {
            string val = "";
            try
            {
                
                foreach (var dataSocket in this.components.Components.OfType<DataSocket>())
                {
                    if (dataSocket.IsConnected)
                        dataSocket.Disconnect();
                    dataSocket.Connect();
                    val += "Url: "+dataSocket.Url + "\n";

                }
                
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void DisposeLab()
        {
            try
            {
                timer.Enabled = false;
                if (scopeTaskComponent != null)
                {
                    scopeTaskComponent.StopTask();
                }

                foreach (var dataSocket in this.components.Components.OfType<DataSocket>())
                {
                    dataSocket.Disconnect();
                    dataSocket.Dispose();
                }
                dataSocketServer.Dispose();
                scopeTaskComponent.Dispose();
                components.Dispose();
                
                //for brevity
                dataSocketServer = null;
                components = null;
                scopeTaskComponent = null;
            }
            catch (Exception)
            {
                //Todo
            }
            
        }
    }
}
