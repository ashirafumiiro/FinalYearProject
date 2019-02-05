﻿using System.Collections.Generic;
using System.Web.Http;
using NationalInstruments.DAQmx;
using System.Windows.Forms;
using System.Linq;

namespace LabServiceLibrary
{
    public class SwitchController : ApiController
    {
        public string Device { get; set; }

        public SwitchController()
        {
            Device = "Dev1";
        }
        // GET api/switch 
        public IEnumerable<string> Get()
        {
            return new string[] { "value3", "value4" };
        }
        
        public string Get(string s1, string s2)
        {
            int s1State = GetState(s1);
            int s2State = GetState(s2);
            LabService.TurnSwitch(0, s1State);
            LabService.TurnSwitch(1, s2State);
            return "OK";
        }
        
        // POST api/values 
        public string Post([FromBody]Switches switches)
        {
            List<string> rets = new List<string>();  // store return values for switchig
            if (isValid(switches.S1))
            {
                rets.Add(LabService.TurnSwitch(0, GetState(switches.S1), Device));
            }
            if (isValid(switches.S2))
            {
                rets.Add(LabService.TurnSwitch(1, GetState(switches.S2), Device));
            }
            if (isValid(switches.S3))
            {
                rets.Add(LabService.TurnSwitch(2, GetState(switches.S3), Device));
            }
            if (isValid(switches.S4))
            {
                rets.Add(LabService.TurnSwitch(3, GetState(switches.S4), Device));
            }

            if (rets.Contains("Fail"))
                return "Fail";
            

            return "OK";
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }

        private int GetState(string state)
        {
            if (state.Equals("on"))
            {
                return 255;
            }
            else
            {
                return 0;
            }
        }

        public bool isValid(string val)
        {
            if (val == null) return false;
            return (val.ToLower().Trim().Equals("on")) || (val.Trim().ToLower().Equals("off"));
        }
    }

    public class Switches
    {
        public string S1 { get; set; }
        public string S2 { get; set; }
        public string S3 { get; set; }
        public string S4 { get; set; }
    }

}