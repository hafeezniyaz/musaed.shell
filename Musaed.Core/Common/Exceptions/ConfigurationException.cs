using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Common.Exceptions
{
    public class ConfigurationException:CoreException
    {
        public ConfigurationException(string message) : base(message)
        {

        }
        public ConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
