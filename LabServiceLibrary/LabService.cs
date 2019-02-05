using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using NationalInstruments.Net;
using System.Windows.Forms;
using System.Media;

namespace LabServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class LabService : ILab
    {
        private double phase = 0;
        System.Timers.Timer timer;

        public LabService()
        {
            InitializeComponent();
            CreateChannels(2);
            CreateScopeTask("Dev1", 2);
            // enitialise time
            timer = new System.Timers.Timer();
            timer.Interval = 1000; // in milli seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
        }

        public string ChangeLab(string labName)
        {
            return "Lab Changed to: " + labName;
        }

        public string GetRunningLab()
        {
            return "Default";
        }

        public string GetState()
        {
            return "Pending";
        }

        public string RunLab()
        {
            return "Lab has started running";
        }

        public string StopLab()
        {

            return DisposeLab();// "Lab Stopped";
        }

        
        public string TestMethod()
        {
            //ConnectDataSockets();
            timer.Enabled = true;
            
            
            return "OK";
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // Get data from task

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
                        SystemSounds.Beep.Play();
                    }
                    i++;
                }


               
            }
            catch (NationalInstruments.DAQmx.DaqException ex)
            {
                timer.Enabled = false;
                MessageBox.Show("In Timer\n"+ex.Message); 
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
                SystemSounds.Beep.Play();
            }
        }

        public string DisposeLab()
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
            return "Lab Stopped";
        }
    }
}
