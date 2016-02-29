using CSharpUtils.StatusLoggerServiceReference;

namespace CSharpUtils.Utils.StatusLogger
{
    public class RemoteStatusLogger : BaseStatusLogger, IStatusLoggerServiceCallback
    {
        public void OnChanged(StatusLoggerServiceReference.StatusChangeEventArgs e)
        {
            Changed(this, StatusChangeEventArgs.FromService(e));
        }
    }
}
