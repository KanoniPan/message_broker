using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageBroker.Hubs;
using MessageBroker.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence;
using Persistence.DBSettings;

namespace MessageBroker
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddSignalR();
            services.Configure<DBSettings>(Configuration.GetSection(nameof(DBSettings)));
            services.AddSingleton<IDBSettings>(sp => sp.GetRequiredService<IOptions<DBSettings>>().Value);
            services.AddSingleton<IDbContext, DbContext>();
            services.AddTransient<IMessageService, Implementation.Implementation>();
            services.AddTransient<ISubscriptionService, Implementation.Implementation>();
            services.AddTransient<IMessageEnricher, Implementation.Implementation>();
            services.AddTransient<IContentBasedRouter, Implementation.Implementation>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseSignalR(options => { options.MapHub<ChatHub>("/api/chat-hub"); });
        }
    }
}