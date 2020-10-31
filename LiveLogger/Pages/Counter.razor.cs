using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using LiveLogger;
using LiveLogger.Shared;
using Microsoft.Extensions.Logging;

namespace LiveLogger.Pages
{
    public partial class Counter
    {
        [Inject]
        ILogger<Counter> logger { get; set; }

        private int currentCount = 0;
        private void IncrementCount()
        {
            currentCount++;
            logger.LogInformation($"Current Count : {currentCount}");
        }
    }
}