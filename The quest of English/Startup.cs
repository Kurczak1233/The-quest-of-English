using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TheEnglishQuestCore;
using TheEnglishQuestCore.Managers;
using TheEnglishQuestDatabase;
using TheEnglishQuestDatabase.Entities;
using TheQuestOfEnglishDatabase;

namespace The_quest_of_English
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
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=FLUTTERSHY\\SQLEXPRESS;Database=TheQuestOfEnglish;Trusted_Connection=True;"));
            services.AddRazorPages().AddRazorRuntimeCompilation();
            //Db
            services.AddTransient<IEncouragementPositionRepository, EncouragementPositionRepository>();
            services.AddTransient<IApplicationUserRepository, ApplicationUserRepository>();
            //Mapper
            services.AddTransient<DTOMapper<ApplicationUser, ApplicationUserDto>>();
            services.AddTransient<DTOMapper<EncouragementPosition, EncouragementPositionDto>>();
            //Main ViewModel
            services.AddTransient<EncouragementPoisitonViewModelMapper>();
            services.AddTransient<ApplicationUserViewModelMapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
            serviceProvider.GetService<ApplicationDbContext>().Database.Migrate();
        }
    }
}
