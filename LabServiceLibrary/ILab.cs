using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace LabServiceLibrary
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

        [OperationContract]
        string TestMethod();

    }
}
