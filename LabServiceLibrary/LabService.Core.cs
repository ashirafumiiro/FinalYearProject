using NationalInstruments.Analysis.SignalGeneration;
using NationalInstruments.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NationalInstruments.DAQmx;
using NationalInstruments.DAQmx.ComponentModel;

namespace LabServiceLibrary
{
    public partial class LabService
    {
        private System.ComponentModel.IContainer components = null;
        private NationalInstruments.DAQmx.Task myTask;
        private DataSocketServer dataSocketServer;
        private List<DataSocket> dataSockets;

        //Scope task config
        private ScopeTaskComponent scopeTaskComponent;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected void Dispose(bool disposing)
        {
            components.Dispose();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Setup dataSocketServer
            this.dataSocketServer = new DataSocketServer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataSocketServer)).BeginInit(); 
           
            ((System.ComponentModel.ISupportInitialize)(this.dataSocketServer)).EndInit();

            this.dataSockets = new List<DataSocket>();

            try
            {
                if (!dataSocketServer.IsOpen)
                {
                    dataSocketServer.Start();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            
        }

        public void CreateChannels(int numberOfChannel)
        {
            for (int i = 1; i <= numberOfChannel; i++)
            {
                DataSocket dataSocket = new DataSocket(this.components);
                ((System.ComponentModel.ISupportInitialize)(dataSocket)).BeginInit();

                ((System.ComponentModel.ISupportInitialize)(dataSocket)).EndInit();

                dataSocket.Url = "dstp://localhost/wave" + i.ToString();
                dataSocket.AccessMode = NationalInstruments.Net.AccessMode.WriteAutoUpdate;
                dataSockets.Add(dataSocket);
         
                
            }
        }

        public void GenerateWave(string device = "Dev1", double freq = 100, double amp = 2,
                         AOFunctionGenerationType type = AOFunctionGenerationType.Sine)
        {
            try
            {
                if (myTask == null)
                {
                    myTask = new NationalInstruments.DAQmx.Task();
                    if (amp < 0)
                    {
                        amp *= -1;
                    }
                    if (amp > 5)
                    {
                        amp = 5;
                    }
                    myTask.AOChannels.CreateFunctionGenerationChannel(device + "/fgen",
                                                                        "fgen",
                                                                        type,
                                                                        freq,  //freq
                                                                        amp);   //amplitude

                    // verify the task before doing the waveform calculations
                    myTask.Control(TaskAction.Verify);
                }
                else
                {
                    myTask.Stop();
                    myTask.AOChannels[0].FunctionGenerationType = type;
                    myTask.AOChannels[0].FunctionGenerationFrequency = freq;
                    myTask.AOChannels[0].FunctionGenerationAmplitude = amp;
                }

                myTask.Start();
            }
            catch (DaqException err)
            {
                MessageBox.Show(err.Message);
                myTask.Dispose();
                myTask = null;
            }
        }

        private void CreateScopeTask(string device, int channels)
        {

            int samplesPerChannel = 1000;
            int rate = 10000;  //sample rate
            double minimumValue = -10;
            double maximumValue = 10;

            if (scopeTaskComponent != null)
            {
                scopeTaskComponent = null;
            }

            this.scopeTaskComponent = 
                new LabServiceLibrary.ScopeTaskComponent(this.components,
                    device, channels, rate, samplesPerChannel, minimumValue,
                    maximumValue);
            ((System.ComponentModel.ISupportInitialize)(this.scopeTaskComponent)).BeginInit();

            ((System.ComponentModel.ISupportInitialize)(this.scopeTaskComponent)).EndInit();
            scopeTaskComponent.Error += scopeTaskComponent_Error;
        }

        void scopeTaskComponent_Error(object sender, ErrorEventArgs e)
        {
            string messege = e.Exception.Message;
            MessageBox.Show("IN Scope Handler\n"+messege);
            scopeTaskComponent.StopTask(); 
            timer.Enabled = false;
        }

        
        private double[] AcquireData(double frequency, double amplitude, double phase, double numberSamples, double noiseAmplitude)
        {
            // Test using signal generation
            SineSignal sineSignal = new SineSignal(frequency, amplitude, phase);
            WhiteNoiseSignal noise = new WhiteNoiseSignal(noiseAmplitude);
            SignalGenerator generator = new SignalGenerator(numberSamples, (long)numberSamples);
            generator.Signals.Add(sineSignal);
            generator.Signals.Add(noise);
            return generator.Generate();
        }

    }
}
