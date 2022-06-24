using System;
using Amazon.S3;
using LightInject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Streamnote.Relational.Data;
using Streamnote.Relational.Helpers;
using Streamnote.Relational.Installers;
using Streamnote.Web.Mapper;
using Streamnote.Relational.Models;
using Streamnote.Web.Messenger;

namespace Streamnote.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("streamnote_db"), 
                    b => b.MigrationsAssembly("Streamnote.Relational")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            // Mappers and helpers.
            services.AddTransient<ProfileMapper>();
            services.AddTransient<ItemMapper>();
            services.AddTransient<CommentMapper>();
            services.AddTransient<UserMapper>();
            services.AddTransient<TopicMapper>();
            services.AddTransient<ProjectMapper>();
            services.AddTransient<TaskMapper>();
            services.AddTransient<StepMapper>();
            services.AddTransient<BlockMapper>();
            services.AddTransient<TaskCommentMapper>();
            services.AddTransient<ImageProcessingHelper>();
            services.AddTransient<DateTimeHelper>();
            services.AddTransient<IAmazonS3>();

            // Connection services.
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
            });
        }

        // Use this method to add services directly to LightInject
        // Important: This method must exist in order to replace the default provider.
        public void ConfigureContainer(IServiceContainer container)
        {
            container.RegisterFrom<RepositoryCompositionRoot>();
            container.RegisterFrom<ServiceCompositionRoot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapHub<SignalRMessenger>("/SignalRMessenger", options =>
                {
                    options.Transports =
                        HttpTransportType.WebSockets |
                        HttpTransportType.LongPolling;
                });
            });
        }
    }
}
