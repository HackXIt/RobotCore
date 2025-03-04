using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnakeMultiplayer.Services;
using SnakeMultiplayer.Components;
using System;
using System.Net.Http;

namespace SnakeMultiplayer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register controllers (for API endpoints)
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        // Register the game service as a hosted background service
        builder.Services.AddHostedService<GameService>();

        // Register HttpClient using NavigationManager to get the base URL.
        builder.Services.AddScoped<HttpClient>(sp =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            Console.WriteLine($"Base URL: {navigationManager.BaseUri}");
            return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
        });

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();

        app.MapControllers();
        app.MapBlazorHub();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
