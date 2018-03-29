#if MXIO

using CSharpUtils.Logging;
using MOXA_CSharp_MXIO;
using System.Net.NetworkInformation;
using System.Text;

namespace CSharpUtils.MXIO
{
    public class MXIOEthernetConnection : MXIOConnection
    {
        /// <summary>
        /// The IP address of the MXIO module.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// The port of the MXIO module.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// Initialize a connection to a new MXIO module.
        /// </summary>
        /// <param name="manager">The MXIO manager that this connection belongs to</param>
        /// <param name="name">The name of the connection</param>
        /// <param name="ip">The IP address of the MXIO module</param>
        /// <param name="port">The port of the MXIO module</param>
        public MXIOEthernetConnection(MXIOManager manager, string name, string ip, ushort port)
            : base(manager, name)
        {
            this.IP = ip;
            this.Port = port;

            Init();
        }

        public override void Open()
        {
            int ret;
            Manager.Logger.Log(LogLevel.INFO, "IP address: {0}", IP);
            Manager.Logger.Log(LogLevel.INFO, "Port: {0}", Port);

            // Ping.
            Ping ping = new Ping();
            PingReply rep;
            rep = ping.Send(IP, (int)Manager.Timeout);
            if (rep.Status != IPStatus.Success)
            {
                throw new MXIOException("Cannot reach the IO module.");
            }

            // Connect.
            int[] conn = new int[1];
            ret = MXIO_CS.MXEIO_Connect(Encoding.UTF8.GetBytes(IP), Port, Manager.Timeout, conn);
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("ConnectMXIO failed: {0}.", ret);
            }
            this.connection = conn[0];

            // Check connection.
            byte[] bytCheckStatus = new byte[1];
            ret = MXIO_CS.MXEIO_CheckConnection(connection, Manager.Timeout, bytCheckStatus);
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("MXIO check connection failed: {0}.", ret);
            }
            if (bytCheckStatus[0] != MXIO_CS.CHECK_CONNECTION_OK)
            {
                string msg;
                switch (bytCheckStatus[0])
                {
                    case MXIO_CS.CHECK_CONNECTION_FAIL:
                        msg = "fail";
                        break;
                    case MXIO_CS.CHECK_CONNECTION_TIME_OUT:
                        msg = "timeout";
                        break;
                    default:
                        msg = "unknown: " + bytCheckStatus[0];
                        break;
                }
                throw new MXIOException("MXIO check connection failed: {0}.", msg);
            }
        }

        public override void Close()
        {
            MXIO_CS.MXEIO_Disconnect(this.connection);
        }
    }
}

#endif