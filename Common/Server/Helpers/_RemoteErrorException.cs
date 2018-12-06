using Common.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Server.Helpers
{
    class RemoteErrorException : Exception
    {
        public RemoteErrorException(ErrorFrame error)
            : base(error.Message)
        {

        }
    }
}
