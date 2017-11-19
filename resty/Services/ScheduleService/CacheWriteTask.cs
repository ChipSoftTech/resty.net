using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using CST.NETCore.SchedulerService.Code;
using System.Diagnostics;
using Serilog.Events;
using System;

namespace resty.Services
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

        const string MessageTemplate = "CacheWriteTask responded {StatusCode} in {Elapsed:0.0000} ms";

        public CacheWriteTask(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var start = Stopwatch.GetTimestamp();
            try
            {
                var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

                await Task.Delay(5000, cancellationToken);

                var level = LogEventLevel.Information;
                Log.Write(level, MessageTemplate, "OK", elapsedMs);
            }
            // Never caught, because `LogException()` returns false.
            catch (Exception ex) when (LogException(GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex)) { }
            
        }

        static bool LogException(double elapsedMs, Exception ex)
        {
            Log.Write(LogEventLevel.Error, ex, MessageTemplate, "Error", elapsedMs);

            return false;
        }

        static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }

    }
}