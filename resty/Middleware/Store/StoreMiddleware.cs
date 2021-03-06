﻿using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace resty
{
    public class StoreMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public StoreMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            Log.Debug("StoreMiddleware starting");

            _cache = cache;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Value.Contains("api"))
            {
                // call next middleware and return (not an api call)
                await _next.Invoke(context);

                return;
            }

            JArray value = getJsonFile(context);

            if(value == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(@"{ ""code"": 404, ""msg"": ""Invalid collection name"" }");
                Log.Information("Invalid collection name");

                return;
            }

            // call next middleware
            await _next.Invoke(context);

            // code executed after the next middleware
            context.Response.StatusCode = 200; //response status 200
            
            await context.Response.WriteAsync(@"{ ""code"": 200, ""msg"": ""OK"", ""data"": " + value.ToString() + "}");

            Log.Debug("StoreMiddleware ending");
        }

        private void handleGetAll(HttpContext context)
        {
            //getJsonFile(context.Request.Query["file"])


        }

        private JArray getJsonFile(HttpContext context)
        {
            // URI split path
            string[] uri;
            string fname = "";
            string cacheValue = "";
            JArray fvalue = null;

            try
            {
                uri = context.Request.Path.ToString().TrimStart('/').Split("/");

                if (uri.Length > 1)
                {
                    //file (store) name
                    fname = uri[1];
                } else
                {
                    //code: 400, msg: 'Invalid Collection name'
                }

                if (Regex.Matches(fname, @"/^[a - zA - Z0 - 9\.-_] +$/").Count == 0)
                {
                    // Fail at invalid file name

                }

                cacheValue = _cache.GetString(fname);

                if (cacheValue != null)
                {
                    try
                    {
                        fvalue = (JArray)JToken.Parse(cacheValue);
                    }
                    catch (Exception e)
                    {
                        //continue to see if pull from file
                        fvalue = null;
                    }
                }

                if (fvalue == null)
                {
                    using (StreamReader reader = File.OpenText(@"data/" + fname + ".json"))
                    {
                        fvalue = (JArray)JToken.ReadFrom(new JsonTextReader(reader));
                    }

                    _cache.SetString(fname, fvalue.ToString());
                }

            }
            catch
            {
                //await context.Response.WriteAsync("error");
            }
            finally
            {
                
            }

            return fvalue; //fvalue;
        }


    }
}
