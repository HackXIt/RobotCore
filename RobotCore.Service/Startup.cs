using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RobotCore.Library.Core;
using RobotCore.Service.Controllers;

namespace RobotCore.Service;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration) => Configuration = configuration;
        
    public void ConfigureServices(IServiceCollection services)
    {
        // Add MVC controllers (or minimal APIs as per your project needs).
        services.AddControllers();
            
        // Register your Robot Library implementation. You could use a concrete implementation that inherits RobotLibraryBase.
        services.AddSingleton<RobotLibraryBase, MyRobotLibrary>();
            
        // Add logging.
        services.AddLogging();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
            
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}