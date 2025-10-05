using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Common.Exceptions
{
   public  class PackageException : CoreException
    {
        public PackageException(string message) : base(message)
        {

        }
        public PackageException(string message, Exception innerException) : base(message, innerException) { }
    }

}
