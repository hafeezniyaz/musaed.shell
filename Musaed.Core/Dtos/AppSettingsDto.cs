using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Dtos
{
    public class AppSettingsDto
    {
        public ServerSettingsDto ServerSettings { get; set; }

        public string? CredentialName { get; set; }
    }
}
