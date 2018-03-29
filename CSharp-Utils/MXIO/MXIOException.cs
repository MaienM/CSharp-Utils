#if MXIO

using MOXA_CSharp_MXIO;
using System;

namespace CSharpUtils.MXIO
{
    public class MXIOException : Exception
    {
        public MXIOException(string message, params object[] args)
            : base(String.Format(message, args))
        {
        }

        public MXIOException(string message, Exception e, params object[] args)
            : base(String.Format(message, args), e)
        {
        }

        public MXIOException(string message, int err, params object[] args)
            : base(String.Format(message, GetErrorMessage(err), args))
        {
        }

        public static string GetErrorMessage(int iRet)
        {
            return System.Enum.GetName(typeof(MXIO_CS.MXIO_ErrorCode), iRet);
        }
    }
}

#endif