using Musaed.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Infrastructure.Handlers
{
    public class AuthenticationHandler: DelegatingHandler
    {
        private readonly IAuthService _authService;

        public AuthenticationHandler(IAuthService authService)
        {
            _authService = authService;
        }


        protected override async Task<HttpResponseMessage> SendAsync
            (
            HttpRequestMessage request , 
            CancellationToken cancellationToken
            )
        {
            var token = await _authService.GetAccessTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers
                    .AuthenticationHeaderValue("Bearer", token);

            }

            return await base.SendAsync(request, cancellationToken);
        }



    }
}
