using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestNET.DATA.Interfaces;
using TestNET.DATA.Repository;
using TestNET.DATA.Scanning;
using TestNET.DATA;

namespace TestNET
{
    public class Startup
    {
        private readonly IConfigurationRoot _connectionString;

        public Startup(IWebHostEnvironment hostEnvironment)
        {
            _connectionString = new ConfigurationBuilder().
                                    SetBasePath(hostEnvironment.ContentRootPath). 
                                    AddJsonFile("DbSettings.json").
                                    Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {     
            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddDbContext<AppDBContent>(options =>
                options.UseSqlServer(_connectionString.GetConnectionString("DefaultConnection")));

            services.AddTransient<IHtmlData, HtmlRepository>();
            services.AddTransient<IScanner, Scanner>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();


            
        }
    }
}
