using System.Collections.ObjectModel;
using System.ServiceModel;

namespace CSharpUtils.Utils.StatusLogger
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class StatusLoggerService : IStatusLoggerService
    {
        private readonly Collection<StatusChangeEventArgs> _log = new SizeLimitedCollection<StatusChangeEventArgs>(1000);
        private readonly Collection<IStatusLoggerCallback> _callbacks = new Collection<IStatusLoggerCallback>();

        public StatusLoggerService(BaseStatusLogger logger)
        {
            logger.Changed += OnChanged;
        }

        public void OpenSession()
        {
            lock (_log)
            {
                // Get and store the callback.
                IStatusLoggerCallback callback = OperationContext.Current.GetCallbackChannel<IStatusLoggerCallback>();
                _callbacks.Add(callback);

                // Send the existing items to the client.
                if (_log == null) return;
                foreach (StatusChangeEventArgs e in _log)
                {
                    callback.OnChanged(e);
                }
            }
        }

        private void OnChanged(object sender, StatusChangeEventArgs e)
        {
            lock (_log)
            {
                // Send the item to all clients.
                foreach (IStatusLoggerCallback callback in _callbacks)
                {
                    callback.OnChanged(e);
                }

                // Store in the log.
                _log.Add(e);
            }
        }
    }
}
