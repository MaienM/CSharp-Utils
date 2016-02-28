using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NamedPipeWrapper;

namespace CSharpUtils.Utils.StatusLogger
{
    public class StatusLoggerServer
    {
        public const string PIPE_NAME = "StatusLogger";

        private readonly NamedPipeServer<StatusChangeEventArgs> _server = new NamedPipeServer<StatusChangeEventArgs>(PIPE_NAME);
        private readonly Collection<NamedPipeConnection<StatusChangeEventArgs, StatusChangeEventArgs>> _connections = new Collection<NamedPipeConnection<StatusChangeEventArgs, StatusChangeEventArgs>>();
        private readonly Collection<StatusChangeEventArgs> _log = new Collection<StatusChangeEventArgs>(); 

        public StatusLoggerServer(LocalStatusLogger logger)
        {
            logger.Changed += this.OnChanged;

            _server.ClientConnected += ClientConnected;
            _server.ClientDisconnected += ClientDisconnected;
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            _server.Start();
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            _server.Stop();
            lock (this)
            {
                _connections.Clear();
            }
        }

        private void OnChanged(object sender, StatusChangeEventArgs e)
        {
            lock (this)
            {
                foreach (NamedPipeConnection<StatusChangeEventArgs, StatusChangeEventArgs> connection in _connections.Where(namedPipeConnection => namedPipeConnection.IsConnected))
                {
                    connection.PushMessage(e);
                }
                _log.Add(e);
            }
        }

        private void ClientConnected(NamedPipeConnection<StatusChangeEventArgs, StatusChangeEventArgs> connection)
        {
            lock (this)
            {
                _connections.Add(connection);

                if (_log == null)
                {
                    return;
                }

                foreach (StatusChangeEventArgs e in _log)
                {
                    connection.PushMessage(e);
                }
            }
        }

        private void ClientDisconnected(NamedPipeConnection<StatusChangeEventArgs, StatusChangeEventArgs> connection)
        {
            lock (this)
            {
                _connections.Remove(connection);
            }
        }
    }
}
