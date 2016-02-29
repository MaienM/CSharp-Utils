using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Utils.StatusLogger
{
    internal interface IStatusLoggerCallback
    {
        [OperationContract]
        void OnChanged(StatusChangeEventArgs e);
    }
}
