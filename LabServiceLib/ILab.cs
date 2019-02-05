using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace LabServiceLib
{
    [ServiceContract(Namespace = "http://ilabsmakerere.com")]
    public interface ILab
    {
        [OperationContract]
        string GetRunningLab();

        [OperationContract]
        string ChangeLab(string labName);

        [OperationContract]
        string StopLab();

        [OperationContract]
        string RunLab();

        [OperationContract]
        string GetState();

    }
}
