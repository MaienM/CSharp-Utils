#if MXIO

using CSharpUtils.Logging;
using MOXA_CSharp_MXIO;
using System.Net.NetworkInformation;
using System.Text;

namespace CSharpUtils.MXIO
{
    public class MXIOE1KConnection : MXIOConnection
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
        public MXIOE1KConnection(MXIOManager manager, string name, string ip, ushort port)
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
            ret = MXIO_CS.MXEIO_E1K_Connect(Encoding.UTF8.GetBytes(IP), Port, Manager.Timeout, conn, new byte[0]);
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("MXEIO_E1K_Connect failed: {0}.", ret);
            }
            this.connection = conn[0];

            // Check connection.
            byte[] bytCheckStatus = new byte[1];
            ret = MXIO_CS.MXEIO_CheckConnection(connection, Manager.Timeout, bytCheckStatus);
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("MXEIO_CheckConnection failed: {0}.", ret);
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
                throw new MXIOException("MXEIO_CheckConnection status failed: {0}.", msg);
            }
        }

        public override void Close()
        {
            MXIO_CS.MXEIO_Disconnect(this.connection);
        }

        public override void SetDigitalOutput(int output, bool value)
        {
            InvokeWithRetry("SetDigitalOutput", () => MXIO_CS.E1K_DO_Writes(connection, (byte)output, 1, (uint)(value ? 1 : 0)));
        }

        public override bool GetDigitalInput(int input)
        {
            uint[] value = new uint[1];
            InvokeWithRetry("GetDigitalInput", () => MXIO_CS.E1K_DI_Reads(connection, 1, (byte)input, value));
            return value[0] == 1;
        }
    }
}

#endif