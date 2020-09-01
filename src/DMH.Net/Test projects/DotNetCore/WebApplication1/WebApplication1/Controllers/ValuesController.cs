using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary1;
using LuckyHome;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication1;

//using Autofac;

namespace LuckyHome
{
    public class DependencyResolverHelpercs
    {
        private readonly IWebHost _webHost;

        /// <inheritdoc />
        public DependencyResolverHelpercs(IWebHost WebHost) => _webHost = WebHost;

        public T GetService<T>()
        {
            using (var serviceScope = _webHost.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    var scopedService = services.GetRequiredService<T>();
                    return scopedService;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            };
        }
    }
    public class PositionOptions
    {
        public const string Position = "Position";

        public string Title { get; set; }
        public string Name { get; set; }
    }

    public class LuckyHomeInterfaceClassMapper
    {

        // private readonly static ILifetimeScope scope;
        //static IConfiguration configuration;
        private static IServiceProvider serviceProvider;
        
        static LuckyHomeInterfaceClassMapper()
        {
            var services = new ServiceCollection();
            // Simple configuration object injection (no IOptions<T>)
            IConfiguration tempIConfiguration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
              .AddUserSecrets("e3dfcccf-0cb3-423a-b302-e3e92e95c128")
              .AddEnvironmentVariables()
              .Build();
            
            services.AddScoped<IConfiguration>(_ => tempIConfiguration);
            services.AddTransient<IframeInterface, FrameInterface>();
            services.Configure<PositionOptions>(tempIConfiguration.GetSection(
                                                    PositionOptions.Position));
            services.AddLogging(builder => builder.AddConsole());
            
            serviceProvider = services.BuildServiceProvider();
        }

        public object Run(Type type)
        {
            object output = serviceProvider.GetService(type);
            if (output == null)
            {
                output = serviceProvider.GetRequiredService(type);
                if (output == null)
                    throw new NotImplementedException();
            }
            return output;
        }
    }
}

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IframeInterface interface1;
        private readonly IConfiguration configuration;
        private readonly IOptions<PositionOptions> positioOption;
        private readonly ILogger<ValuesController> logger;

        public ValuesController(IframeInterface interface1, IConfiguration configuration, IOptions<PositionOptions> positioOption,
            ILogger<ValuesController> logger)
        {
            this.interface1 = interface1;
            this.configuration = configuration;
            this.positioOption = positioOption;
            this.logger = logger;
        }

        [NonAction]
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                //.SetBasePath(outputPath)
                .AddJsonFile("WebApplication1.deps.json", optional: true)
                .AddUserSecrets("e3dfcccf-0cb3-423a-b302-e3e92e95c128")
                .AddEnvironmentVariables()
                .Build();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            logger.LogInformation("Getting item {Id}", -1);
            //var data = GetIConfigurationRoot();
            return new string[] { "value1", "value2", configuration["SampleAppSettings"], positioOption.Value.Name };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            logger.LogInformation("Getting item {Id}", id);
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
