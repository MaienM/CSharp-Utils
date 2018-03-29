#if MXIO

using System;
using CSharpUtils.Logging;
using MOXA_CSharp_MXIO;

namespace CSharpUtils.MXIO
{
    public class MXIOComConnection : MXIOConnection
    {
        /// <summary>
        /// The COM port of the MXIO module.
        /// </summary>
        public int COM { get; set; }

        /// <summary>
        /// The unit id of the MXIO module.
        /// </summary>
        public int UnitID { get; set; }

        /// <summary>
        /// The transmission format to communicate with the MXIO module.
        /// </summary>
        public short TransmissionMode { get; set; }
            
        /// <summary>
        /// Initialize a connection to a new MXIO module.
        /// </summary>
        /// <param name="manager">The MXIO manager that this connection belongs to</param>
        /// <param name="name">The name of the connection</param>
        /// <param name="com">The COM port of the MXIO module</param>
        /// <param name="unitId">The unit id of the MXIO module</param>
        /// <param name="transmissionMode">The transmission format to communicate with the MXIO module</param>
        public MXIOComConnection(MXIOManager manager, string name, int com, int unitId, short transmissionMode = MXIO_CS.MODBUS_RTU)
            : base(manager, name)
        {
            this.COM = com;
            this.UnitID = unitId;
            this.TransmissionMode = transmissionMode;

            Init();
        }

        public override void Open()
        {
            int ret;
            Manager.Logger.Log(LogLevel.INFO, "COM port: {0}", COM);
            Manager.Logger.Log(LogLevel.INFO, "Unit id: {0}", UnitID);
            Manager.Logger.Log(LogLevel.INFO, "Transmission mode: {0}", TransmissionMode);

            // Connect.
            int[] conn = new int[1];
            ret = MXIO_CS.MXSIO_Connect(COM, (byte)UnitID, (byte)TransmissionMode, conn);
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("ConnectMXIO failed: {0}.", ret);
            }
            this.connection = conn[0];
        }

        public override void Close()
        {
            MXIO_CS.MXSIO_Disconnect(this.connection);
        }
    }
}

#endif