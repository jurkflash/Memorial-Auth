using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sso.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;
        private readonly IIdentityServerInteractionService _interaction;

        public ErrorModel(
            ILogger<ErrorModel> logger,
            IIdentityServerInteractionService interaction
            )
        {
            _logger = logger;
            _interaction = interaction;
        }

        public void OnGet(string errorId)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var t = _interaction.GetErrorContextAsync(errorId);
        }
    }
}
