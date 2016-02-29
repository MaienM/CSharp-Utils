namespace CSharpUtils.Utils.StatusLogger
{
    public class RemoteStatusLogger : BaseStatusLogger, IStatusLoggerCallback
    {
        public void OnChanged(StatusChangeEventArgs e)
        {
            Changed(this, e);
        }
    }
}
