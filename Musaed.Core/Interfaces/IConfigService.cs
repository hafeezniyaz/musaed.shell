using Musaed.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Interfaces
{
    /// <summary>
    /// config interface to provide access to app config
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// get the app configuration
        /// </summary>
        AppConfig Config { get; }
    }
}
