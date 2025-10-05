using Musaed.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Interfaces
{
    public interface IPackageManager
    {
        string ExePath { get;}
        /// <summary>
        /// validate if package available to run , if not download and extract
        /// </summary>
        /// <param name="serverSettings"></param>
        /// <returns></returns>
        Task EnsurePackageIsReadyAsync(Guid appId, ServerSettingsDto serverSettings);

    }
}
