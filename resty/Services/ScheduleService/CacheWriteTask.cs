using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using AppWithScheduler.Code;

namespace AppWithScheduler.Code
{
    public class CacheWriteTask : IScheduledTask
    {
        //minute (0-59), hour (0-23), day of month (1-31), month (1-12), day of week (0-6, Sunday = 0)
        //run every minute = * * * * *
        //run every hour at 0 & 10 & 20 = 0,10,20 * * * *
        //run every 5 minutes = */5 * * * *
        //run once an our between 5am & 10 am = 0 5-10 * * *     
        public string Schedule => "* * * * *";

        private readonly IDistributedCache _cache;

        public CacheWriteTask(IDistributedCache cache)
        {
            Log.Information("CacheWriteTask starting");

            _cache = cache;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log.Information("CacheWriteTask Task Starting");

            await Task.Delay(5000, cancellationToken);
        }
    }
}