using System.Collections.Generic;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Services;
using L4DStatsApi.Support;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace L4DStatsApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private const string API_TITLE = "L4D Custom Player Statistics API";
        private bool swagger = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnv"></param>
        public Startup(IHostingEnvironment hostEnv)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnv.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{hostEnv.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (hostEnv.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(true);
            }

            Configuration = builder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IStatsService, StatsServiceMock>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = Configuration["IdentityService:ValidIssuer"],
                        ValidAudience = Configuration["IdentityService:ValidAudience"],
                        IssuerSigningKey = JwtSecurityKey.Create(Configuration["IdentityService:IssuerSigningKey"])
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("GameServer",
                    policy => policy.RequireClaim("GameServerIdentifier"));
            });
            
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add framework services.
            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            string xmlComments = GetXmlCommentsPath();

            if (!System.IO.File.Exists(xmlComments))
            {
                return;
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ui",
                    new Info
                    {
                        Title = API_TITLE,
                        Description = "An API for storing and loading L4D player statistics.",
                        Version = "v1",
                        TermsOfService = "None",
                        Contact = new Contact
                        {
                            Email = "stranger@pilssi.net",
                            Name = "Mikko Andersson",
                            Url = "http://localhost"
                        }
                    });

                c.IncludeXmlComments(xmlComments);
                c.DescribeAllEnumsAsStrings();

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(security);
            });

            this.swagger = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();

            if (!this.swagger)
            {
                return;
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/ui/swagger.json", API_TITLE);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetXmlCommentsPath()
        {
            var app = PlatformServices.Default.Application;
            return System.IO.Path.Combine(app.ApplicationBasePath, string.Format("{0}.xml", app.ApplicationName));
        }
    }
}
