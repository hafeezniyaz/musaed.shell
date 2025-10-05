using Musaed.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Interfaces
{
    public interface IProcessManager
    {
        /// <summary>
        /// Start the local server 
        /// </summary>
        /// <returns></returns>
        Task StartServerAsync(AppSettingsDto appSettings, Guid appId);

        /// <summary>
        /// 
        /// </summary>
        Task StopServer(bool? closeAllInstancedAndServer=false);
    }
}
