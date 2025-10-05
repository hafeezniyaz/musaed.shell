using Musaed.Core.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.Application.Common.Exceptions
{
    public class NotFoundException : CoreException
    {
        public NotFoundException(string message) : base(message)
        {
            
        }
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
