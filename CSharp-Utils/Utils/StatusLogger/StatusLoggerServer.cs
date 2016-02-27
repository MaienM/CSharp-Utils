using System;
using System.IO;

namespace CSharpUtils.Utils.StatusLogger
{
    class StatusLoggerServer
    {
        public static int Port { get; set; }

        public StatusLoggerServer(StatusLogger logger, int port)
        {
            Port = port;
            logger.Changed += this.OnChanged;
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
        }

        private void OnChanged(object sender, StatusChangeEventArgs e)
        {
        }
    }
}
