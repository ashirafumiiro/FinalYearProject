using System.Collections.Generic;
using System.Web.Http;
using NationalInstruments.DAQmx;
using System.Windows.Forms;
using System.Linq;

namespace LabServiceLibrary
{
    public class ValuesController : ApiController
    {
        public string Device { get; set; }

        public ValuesController()
        {
            Device = "Dev1";
        }

         //GET api/values 
        public IEnumerable<string> Get()
        {
            return new string[] { "default", "value", Device };
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        public string Get(int id, double freq, double amp)
        {
            return LabService.GenerateWave(freq: freq, amp: amp);
        }

        // POST api/values 
        public string Post([FromBody]Signal signal)
        {
            string status = "Fail";  //if signal is not valid, failed
            bool state = true; // assume signal is in right state

            if (signal.Amplitude == null || signal.Frequency==null || signal.WaveType == null)
            {
                state = false;
            }
            else if (signal.Amplitude>5 || signal.Frequency>1000000 ||
                signal.Amplitude < 0 || signal.Frequency < 0) // frequency less than 1M
            {
                state = false;
            }
            
            if (state)
            {
                status = LabService.GenerateWave(
                    freq: signal.Frequency, 
                    amp: signal.Amplitude,
                    waveType: signal.WaveType, sourceDevice: Device); 
            }
             
            return status;
        }

        // PUT api/values/5 
        public string Put(int id, [FromBody]string  value)
        {
            return value;
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }

    public class Signal
    {
        public double Amplitude { get; set; }
        public double Frequency { get; set; }
        public string WaveType { get; set; }

        public override string ToString()
        {
            return "Amp: "+Amplitude.ToString()+
                ", Freq: "+ Frequency.ToString() + ", Wave: "+ WaveType;
        }
    }
}