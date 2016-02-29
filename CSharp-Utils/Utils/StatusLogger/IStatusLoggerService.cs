using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CSharpUtils.Utils.StatusLogger
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IStatusLoggerService" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IStatusLoggerCallback))]
    public interface IStatusLoggerService
    {
        [OperationContract]
        void OpenSession();
    }
}
