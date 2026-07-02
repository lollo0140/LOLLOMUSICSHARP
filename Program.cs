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

class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Aggiungi il supporto per i controller API (se ti servono)
        builder.Services.AddControllers();

        // Configura Electron
        builder.WebHost.UseElectron(args);

        var app = builder.Build();

        // Serviamo i file statici di Svelte (quando l'app è compilata)
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthorization();

        app.MapControllers();

        // Gestione dello sviluppo: reindirizza le chiamate al server Vite di Svelte
        if (app.Environment.IsDevelopment())
        {
            // Se Electron è attivo, avvialo puntando al server locale di Svelte
            if (HybridSupport.IsElectronActive)
            {
                CreateElectronWindow();
            }
        }
        else
        {
            // In produzione, se Electron è attivo, avvialo normalmente
            if (HybridSupport.IsElectronActive)
            {
                CreateElectronWindow();
            }
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
