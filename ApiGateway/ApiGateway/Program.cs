using AspNetCoreRateLimit;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure JWT Authentication with hardcoded secret
var secretKey = "eb9699e0506f7915eeb7cd49fb2f3a586951dc6b3e0bd9592cc1358060af29aa2aa721c23c45ee17dec0ce4423a9376f53a1838eac781d23a8037e8088d84fef5286b4d3644e22c5b8e8d15d9e52b491adf18ee59c212ae53c89e46191be080be0bf330f5ea46be6c00da15aa8b505553cddc80ad8afa0d550521c42a265066ec735e9f938e7fa7c27e4c8de30a3b39f5cb0cd8b7c30028b3580f7b6a5cdff3bc801fbc70165a00d8f96ed36323bc0c63986f2e3e2630e129c4dfdce8807317252c89b1870e26dff711ecb030cc4b77385daed57570faf39490337ef152af06c165c39400bb30aa9cea4894fa80d387d4400c8c9fe45bbfbf9124bd7f1ccde5e!";  // hardcoded secret
var key = Encoding.ASCII.GetBytes(secretKey);



// Add YARP proxy
builder.Services.AddReverseProxy()
    .LoadFromMemory(new[]
    {
        new Yarp.ReverseProxy.Configuration.RouteConfig()
        {
            RouteId = "productRoute",
            ClusterId = "productCluster",
            Match = new Yarp.ReverseProxy.Configuration.RouteMatch
            {
                Path = "/api/products/{**catch-all}"
            }
        },
        new Yarp.ReverseProxy.Configuration.RouteConfig()
        {
            RouteId = "orderRoute",
            ClusterId = "orderCluster",
            Match = new Yarp.ReverseProxy.Configuration.RouteMatch
            {
                Path = "/api/orders/{**catch-all}"
            }
        }
    },
    new[]
    {
        new Yarp.ReverseProxy.Configuration.ClusterConfig()
        {
            ClusterId = "productCluster",
            Destinations = new Dictionary<string,  Yarp.ReverseProxy.Configuration.DestinationConfig>
            {
                { "productDestination", new  Yarp.ReverseProxy.Configuration.DestinationConfig { Address = "http://localhost:5236/" } }
            }
        },
        new Yarp.ReverseProxy.Configuration.ClusterConfig()
        {
            ClusterId = "orderCluster",
            Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
            {
                { "orderDestination", new Yarp.ReverseProxy.Configuration.DestinationConfig { Address = "http://localhost:5230/" } }
            }
        }
    });

// Add rate limiting services (AspNetCoreRateLimit)
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 5,           // max 5 requests
            Period = "1m"        // per 1 minute
        }
    };
});
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Add health checks for gateway itself
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

// Add health check for downstream services
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri("http://localhost:5236/health"), "Product Service")
    .AddUrlGroup(new Uri("http://localhost:5230/health"), "Order Service");

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseIpRateLimiting();


app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();

    // Health check endpoint
    endpoints.MapHealthChecks("/health");
});

app.Run();