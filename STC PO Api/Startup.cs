using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using STC_PO_Api.Context;
using STC_PO_Api.Services;

namespace STC_PO_Api
{
    public class Startup
    {

        private IConfiguration _configuration;
        private IWebHostEnvironment _env;

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();

            services.AddDbContext<POContext>(options => options.UseSqlServer(_configuration.GetConnectionString("POContext")));
            services.AddDbContext<POGuidStatusContext>(options => options.UseSqlServer(_configuration.GetConnectionString("POGuidStatus")));

            services.AddScoped<IPOPendingData, SqlPOPendingData>();
            services.AddScoped<IPOAuditTrailData, SqlPOAuditTrailData>();
            services.AddScoped<IPOApproverData, SqlPOApproverData>();
            services.AddScoped<IPOGuidStatusData, SqlPOGuidStatusData>();
            services.AddScoped<IPOSupplierData, SqlPOSupplierData>();
            services.AddScoped<IPOAttachmentData, SqlPOAttachment>();
            services.AddScoped<IPOExternalAttachmentData, SqlPOExternalAttachment>();
            services.AddScoped<ILinkActionData, SqlLinkActionData>();

            services.AddTransient<IUtils, Utils>();

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


            services.AddCors(options =>
            {
                    options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000",
                                            "https://192.168.0.222")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });
            //config mvc
            services.AddMvc()
                .AddMvcOptions(mvcOptions =>
                {
                    //add formatter for xml
                    //mvcOptions.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    mvcOptions.EnableEndpointRouting = false;
                    //require https only
                    //mvcOptions.Filters.Add(typeof(RequireHttpsAttribute));
                })
               .AddNewtonsoftJson(options =>
               {
                   options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
               });

        }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "po")),
                RequestPath = "/POPdf"
            });
            app.UseCors(MyAllowSpecificOrigins);
            app.UseStatusCodePages();
            app.UseMvc();
        }
    }
}
