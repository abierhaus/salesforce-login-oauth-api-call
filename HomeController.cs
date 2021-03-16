using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using salesforce_login_oauth_api_call;

namespace salesforce_platform_events_dotnetcore
{
    public class HomeController : Controller
    {
        public ISalesforceService SalesforceService { get; set; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ISalesforceService salesforceService)
        {
            _logger = logger;
            SalesforceService = salesforceService;
        }


        public async Task<IActionResult> Index()
        {

            var result = await SalesforceService.TestCallAsync();

            _logger.LogInformation(result);

            return new OkObjectResult(result);
        }
    }
}