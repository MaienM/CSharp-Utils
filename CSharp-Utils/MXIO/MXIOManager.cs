#if MXIO

using System.Collections.Generic;
using CSharpUtils.Logging;
using MOXA_CSharp_MXIO;

namespace CSharpUtils.MXIO
{
    /// <summary>
    /// Create and manager connections with one or more MXIO IO modules.
    /// 
    /// This builds on the default MXIO libraries, as provided by the manufacturer: https://www.moxa.com/product/MXIO_Library.htm.
    /// 
    /// In order for this to work, the MXIO.cs and MXIO_NET.dll files must be placed in the Vendor directory, and the compilation symbol "MXIO" must be added.
    /// </summary>
    public class MXIOManager
    {
        /// <summary>
        /// The logger to be used.
        /// </summary>
        public Logger Logger { get; set; }

        /// <summary>
        /// The amount of times to retry actions, to account for network instability.
        /// </summary>
        public short Retries { get; set; } = 10;

        /// <summary>
        /// The time between retries 
        /// </summary>
        public int RetryDelay { get; set; } = 100;

        /// <summary>
        /// The timeout for actions to the MXIO module.
        /// </summary>
        public uint Timeout { get; set; } = 500;

        /// <summary>
        /// The MXIO connections.
        /// </summary>
        private List<MXIOConnection> connections = new List<MXIOConnection>();

        public MXIOManager(Logger logger)
        {
            this.Logger = logger;
            Logger.Log(LogLevel.INFO, "Initializing the MXIO library...");
            int ret;

            // DLL version.
            ret = MXIO_CS.MXIO_GetDllVersion();
            Logger.Log(LogLevel.INFO, "DLL Version: {0}.{1}.{2}.{3}", (ret >> 12) & 0xF, (ret >> 8) & 0xF, (ret >> 4) & 0xF, (ret) & 0xF);

            // Build date.
            ret = MXIO_CS.MXIO_GetDllBuildDate();
            Logger.Log(LogLevel.INFO, "DLL Build Date: {0:x}/{1:x}/{2:x}", (ret >> 16), (ret >> 8) & 0xFF, (ret) & 0xFF);

            // Init.
            ret = MXIO_CS.MXEIO_Init();
            if (ret != MXIO_CS.MXIO_OK)
            {
                throw new MXIOException("MXIO init failed: {0}.", ret);
            }
        }

        /// <summary>
        /// Connect to an MXIO module.
        /// </summary>
        /// <param name="name">The name of the MXIO module, for logging purposes</param>
        /// <param name="ip">The IP address of the MXIO module</param>
        /// <param name="port">The port of the MXIO module</param>
        public MXIOConnection Connect(string name, string ip, ushort port)
        {
            MXIOConnection connection = new MXIOE1KConnection(this, name, ip, port);
            this.connections.Add(connection);
            return connection;
        }
    }
}

#endif