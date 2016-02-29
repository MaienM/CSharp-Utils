using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Utils.StatusLogger
{
    internal interface IStatusLoggerCallback
    {
        void OnChanged(StatusChangeEventArgs e);
    }
}
