using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace LabServiceLib
{
    public class LabService : ILab
    {
        public LabService()
        {
            Console.WriteLine("Lab Service");
        }

        public string ChangeLab(string labName)
        {
            return "Lab Changed to: "+labName;
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
            return "Lab Stopped";
        }

        //[OperationContract]
        public string TestMethon()
        {
            return "Test Method Called";
        }
    }
}
