#if MXIO

using CSharpUtils.Logging;
using MOXA_CSharp_MXIO;
using System;
using System.Threading;

namespace CSharpUtils.MXIO
{
    public abstract class MXIOConnection
    {
        /// <summary>
        /// The MXIO manager this connection belongs to.
        /// </summary>
        public MXIOManager Manager { get; set; }

        /// <summary>
        /// The name of the connection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The connection handle.
        /// </summary>
        protected int connection;

        /// <summary>
        /// Initialize a connection to a new MXIO module.
        /// </summary>
        /// <param name="manager">The MXIO manager that this connection belongs to</param>
        /// <param name="name">The name of the connection</param>
        public MXIOConnection(MXIOManager manager, string name)
        {
            this.Manager = manager;
            this.Name = name;
        }

        protected void Init()
        { 
            Manager.Logger.Log(LogLevel.INFO, "Connecting to the IO module for {0}...", Name);

            // Connect.
            Open();

            // Get firmware version
            int ret;
            byte[] bytRevision = new byte[4];
            ret = MXIO_CS.MXIO_ReadFirmwareRevision(connection, bytRevision);
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("MXIO read firmware version failed: {0}.", ret);
            }
            Manager.Logger.Log(LogLevel.INFO, "Firmware Version: {0}.{1}, release: {2}, build: {3}", bytRevision[0], bytRevision[1], bytRevision[2], bytRevision[3]);

            // Get firmware release date
            ushort[] wGetFirmwareDate = new ushort[2];
            ret = MXIO_CS.MXIO_ReadFirmwareDate(connection, wGetFirmwareDate);
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("MXIO read firmware date failed: {0}.", ret);
            }
            Manager.Logger.Log(LogLevel.INFO, "Firmware Date: {0:x}/{1:x}/{2:x}", wGetFirmwareDate[1], (wGetFirmwareDate[0] >> 8) & 0xFF, (wGetFirmwareDate[0]) & 0xFF);

            // Get module type
            ushort[] wModuleType = new ushort[1];
            ret = MXIO_CS.MXIO_GetModuleType(connection, 0, wModuleType);
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("MXIO get module type failed: {0}.", ret);
            }
            Manager.Logger.Log(LogLevel.INFO, "Module type: {0:x}", wModuleType[0]);
        }

        /// <summary>
        /// (Re)connect to the MXIO module.
        /// </summary>
        abstract public void Open();

        /// <summary>
        /// Disconnect from the MXIO module.
        /// </summary>
        abstract public void Close();

        private void InvokeWithRetry(string methodName, Func<int> method)
        {
            int attempt = 0;
            while (true)
            {
                int ret = method.Invoke();
                if (ret == MXIO_CS.MXIO_OK)
                {
                    return;
                }

                attempt++;
                if (attempt < Manager.Retries)
                {
                    Manager.Logger.Log(LogLevel.DEBUG, "{} failed ({}) on module {}, re-trying ({}/{})", methodName, MXIOException.GetErrorMessage(ret), Name, attempt, Manager.Retries);
                    Thread.Sleep(Manager.RetryDelay);
                    Open();
                }
                else
                {
                    throw new MXIOException("{} failed on {}: {}", methodName, Name, MXIOException.GetErrorMessage(ret));
                }
            }
        }
        /// <summary>
        /// Reset the MXIO module.
        /// </summary>
        public void Reset()
        {
            InvokeWithRetry("Reset", () => MXIO_CS.MXIO_Reset(connection));
        }

        /// <summary>
        /// Set an output of the MXIO module.
        /// </summary>
        /// <param name="output">The number of the output to set</param>
        /// <param name="status">The state to set it to</param>
        public void SetDigitalOutput(int output, bool value)
        {
            InvokeWithRetry("SetOutput", () => MXIO_CS.DO_Write(connection, 1, (byte)output, (byte)(value ? 1 : 0)));
        }

        /// <summary>
        /// Get an input of the MXIO module.
        /// </summary>
        /// <param name="input">The number of the input to get</param>
        private bool GetDigitalInput(int input)
        {
            byte[] value = new byte[1];
            InvokeWithRetry("GetInput", () => MXIO_CS.DI_Read(connection, 1, (byte)input, value));
            return value[0] == 1;
        }
    }
}

#endif