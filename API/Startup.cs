using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Exstensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config ;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_config);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
            });

            services.AddCors();
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }

            // Redirect to HTTPS end point if using HTTP 
            app.UseHttpsRedirection();

            // Use routing to route to defined routes
            app.UseRouting();

            // UseCors must be configured between UseRouting and UseAuthorization
            // Tell browser that http://localhost:4200/ origin is allowed to request from API with any header passed
            // for any method (get put ....)
            // Response header contains access-control-allow-origin: http://localhost:4200
            // So Server allows http://localhost:4200 to request. For That chrome will not block response 
            // because the server passed access-control-allow-origin: http://localhost:4200 saying server allows requests
            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(new string[2]{"http://localhost:4200", "https://localhost:4200"}));

            // must be after app.UseCors
            app.UseAuthentication();

            // must be after app.UseAuthentication
            app.UseAuthorization();

            // Use endpoints defined in controller
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
