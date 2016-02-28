using System;
using NamedPipeWrapper;

namespace CSharpUtils.Utils.StatusLogger
{
    public class RemoteStatusLogger : BaseStatusLogger
    {
        private readonly NamedPipeClient<StatusChangeEventArgs> _client = new NamedPipeClient<StatusChangeEventArgs>(StatusLoggerServer.PIPE_NAME);

        public RemoteStatusLogger()
        {
            _client.ServerMessage += ServerMessage;
        }

        public void Start()
        {
            _client.Start();
            _client.WaitForConnection();
        }

        public void Stop()
        {
            _client.Stop();
        }

        private void ServerMessage(NamedPipeConnection<StatusChangeEventArgs, StatusChangeEventArgs> connection, StatusChangeEventArgs message)
        {
            Changed(this, message);
        }
    }
}
