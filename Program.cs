using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Threading;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Aggiungi il supporto per i controller API (se ti servono)
        builder.Services.AddControllers();

        // Configura Electron
        builder.WebHost.UseElectron(args);

        builder.Services.AddSingleton<IpcMain.AudioHandler>();

        var app = builder.Build();


        app.MapGet("/api/audio/{id}", async (string id, IpcMain.AudioHandler handler) =>
        {
            string? path = await handler.GetAudioData(id);
            if (path == null) return Results.NotFound();

            return Results.File(path, contentType: "audio/webm", enableRangeProcessing: true);
        });

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthorization();

        app.MapControllers();

        if (HybridSupport.IsElectronActive)
        {
            Electron.App.CommandLine.AppendSwitch("disable-gpu");
            Electron.App.CommandLine.AppendSwitch("disable-gpu-compositing");

            CreateElectronWindow();
        }


        app.Run();

    }


    static async void CreateElectronWindow()
    {


        string preloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "preload.js");

        if (!File.Exists(preloadPath))
        {
            preloadPath = Path.Combine(Directory.GetCurrentDirectory(), "preload.js");
        }



        var options = new BrowserWindowOptions
        {
            Frame = false,
            Show = false,
            Transparent = true,
            Resizable = false,
            Movable = false,
            SkipTaskbar = true,
            AlwaysOnTop = false,
            WebPreferences = new WebPreferences
            {
                ContextIsolation = true,
                NodeIntegration = false,
                BackgroundThrottling = true,
                Offscreen = false,
                Preload = preloadPath
            }
        };

        var Window = await Electron.WindowManager.CreateWindowAsync(options);





        IpcMain.RegisterEvents(Window);
        IpcMain.RegisterHandlers(Window);

        Window.LoadURL("http://localhost:5173/");

        Window.OnReadyToShow += () => Window.Show();

        FlyoutWindow.setWinOpenedPosition(Window);
    }




}
