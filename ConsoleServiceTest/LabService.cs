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
using System.Xml.Linq;

namespace LabServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public partial class LabService 
    {
        private int numOfErrors = 0;
        int numberOfChannels = 2;
        System.Timers.Timer timer;

        string workingPath =
            System.IO.Path.Combine(Environment
            .GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "LabServer");  //test =>"halfwave.xml"

        public LabService()
        {          
            StartWebServer();
            // enitialise time
            timer = new System.Timers.Timer();
            timer.Interval =500; // in milli seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
    
        }

        public bool CreatedComponents()
        {
            InitializeComponent();
            if (!LoadCurrentFile(workingPath + "/Labs/TestLab.xml"))
                return false;

            return true;          
        }
     
        
        public void Start()
        {
            if (CreatedComponents())
            {
                timer.Enabled = true;
            }
            else
            {
                //to do. report erro in xml
            }
            
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

                int i = 0;  //channels maintain order with dataSockets in list
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
                MessageBox.Show(ex.Message, "Timer Error", MessageBoxButtons.OK);
                numOfErrors++;
            }
        }

        public void ConnectDataSockets()
        {
            try
            {
                
                foreach (var dataSocket in this.components.Components.OfType<DataSocket>())
                {
                    if (dataSocket.IsConnected)
                        dataSocket.Disconnect();
                    dataSocket.Connect();

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

        public bool LoadCurrentFile(string path)
        {
            try
            {
                XDocument labDoc = XDocument.Load(path);

                string deviceName = (from dev in labDoc.Descendants("Setting")
                                     where (string)dev.Attribute("Name") == "Device"
                                     select (string)dev.Attribute("Value").Value).FirstOrDefault();

                string hostAddress = (from dev in labDoc.Descendants("Setting")
                                      where (string)dev.Attribute("Name") == "Lab Url"
                                     select (string)dev.Attribute("Value").Value).FirstOrDefault();

                List<Channel> channels = (from channel in labDoc.Descendants("Setting")
                                          where (string)channel.Attribute("Type").Value == "Channel"
                                          select new Channel
                                          {
                                              Name = channel.Attribute("Name").Value,
                                              Url = channel.Attribute("Value").Value,
                                              DevicePath = channel.Attribute("DevicePath").Value
                                          }
                               ).ToList<Channel>();
                this.numberOfChannels = channels.Count;
                string val = string.Format("Device Name: {0}\n", deviceName);
                foreach (Channel item in channels)
                {
                    val += "Channel Name: " + item.Name + ", Url: " + item.Url + "\n";
                }
                val += "Lab Url: " + hostAddress;

                MessageBox.Show(val);

                CreateChannels(channels, hostAddress);
                CreateScopeTask(deviceName, channels);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
            
        }
    }

    public class Channel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string DevicePath { get; set; }
    }

}
