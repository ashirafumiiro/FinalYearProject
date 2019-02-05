using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using LabServiceLibrary;
using Topshelf;


namespace ConsoleServiceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<LabService>(s => 
                {
                    s.ConstructUsing(labService => new LabService());
                    s.WhenStarted(lS => lS.Start());
                    s.WhenStopped(lS => lS.Stop());
                });

                x.RunAsLocalSystem();                                       

                x.SetDescription("Lab server service for Hosting the lab Currently being done");                   
                x.SetDisplayName("Lab Server Service");                                  
                x.SetServiceName("LabServerService");                                  
            });

            var exitCodeVal = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());  
            Environment.ExitCode = exitCodeVal;
        }

    }

}
