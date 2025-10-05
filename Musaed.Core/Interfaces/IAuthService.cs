using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Interfaces
{
    public interface IAuthService
    {
         /// <summary>
         /// Get accesss if you are using OAuth 
         /// </summary>
         /// <returns></returns>
        Task<string> GetAccessTokenAsync();

    }
}
