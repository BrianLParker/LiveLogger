using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace LiveLogger.Pages
{
    public partial class Counter
    {
        [Inject]
        private ILogger<Counter> logger { get; set; }

        private int currentCount = 0;
        private void IncrementCount()
        {
            this.currentCount++;
            logger.LogInformation($"Current Count : {this.currentCount}");
        }
    }
}