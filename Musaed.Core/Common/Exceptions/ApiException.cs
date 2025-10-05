using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Common.Exceptions
{
    class ApiException: CoreException
    {
        public ApiException(string message) : base(message)
        {

        }
        public ApiException(string message, Exception innerException) : base(message, innerException) { }
    }
}
