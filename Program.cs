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
using YoutubeMusic;
using System.Drawing;

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


            var settingsFilePath = JsonNode.Parse(File.ReadAllText(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "settings.json")));


            string dowPath = handler.GetYTClient().downloadPath;

            string? pathFromSettings = settingsFilePath?["localData"]?["downloadPath"].GetValue<string>() ?? null;

            if (pathFromSettings != null && pathFromSettings != "none")
            {
                string P = Path.Join(pathFromSettings, $"{id}.webm");

                if (File.Exists(P))
                {
                    return Results.File(P, contentType: "audio/webm", enableRangeProcessing: true);
                }
            }



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

            if (!app.Environment.IsDevelopment())
            {
                Electron.App.CommandLine.AppendSwitch("js-flags", "--max-old-space-size=128");
                Electron.App.CommandLine.AppendSwitch("disable-renderer-backgrounding");
                Electron.App.CommandLine.AppendSwitch("renderer-process-limit", "1");
                Electron.App.CommandLine.AppendSwitch("disk-cache-size", "20971520");
                Electron.App.CommandLine.AppendSwitch("disable-extensions");
            }



            CreateElectronWindow(app);
        }

        createSettingsFile();

        app.Run();

    }

    static void CreateTray(WebApplication app, BrowserWindow window)
    {
#if WINDOWS
        Electron.App.Ready += async () =>
        {
            // 1. Crea la finestra principale
            var window = await Electron.WindowManager.CreateWindowAsync();

            // 2. Definisci gli elementi del menu contestuale (tasto destro)
            var menuItems = new MenuItem[]
            {
                new MenuItem
                {
                    Label = "Mostra App",
                    Click = () => window.Show()
                },
                new MenuItem
                {
                    Label = "Nascondi",
                    Click = () => window.Hide()
                },
                new MenuItem
                {
                    Type = MenuType.separator
                },
                new MenuItem
                {
                    Label = "Esci",
                    Click = () => Electron.App.Quit()
                }
            };

            // 3. Inizializza l'icona e il tooltip della Tray
            // Nota: Il percorso dell'icona parte dalla root della directory di output del progetto
            string iconPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Icon.png");

            await Electron.Tray.Show(iconPath, menuItems);
            await Electron.Tray.SetToolTip("La mia App .NET con Electron");

        };


#endif
    }

    static void createSettingsFile()
    {
        string settingsFilePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        if (!File.Exists(settingsFilePath))
        {
            JsonObject settings = new JsonObject
            {
                ["localData"] = new JsonObject
                {
                    ["downloadPath"] = "none"
                }
            };

            File.WriteAllText(settingsFilePath, JsonSerializer.Serialize(settings));

        }



    }

    static async void CreateElectronWindow(WebApplication app)
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
            Movable = true,
            SkipTaskbar = true,
            AlwaysOnTop = true,
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
        //Window.LoadURL($"http://localhost:{BridgeSettings.WebPort}/");


        Window.OnMinimize += async () =>
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        };

        CreateTray(app, Window);

        Window.OnReadyToShow += () => Window.Show();

        FlyoutWindow.setWinOpenedPosition(Window);
    }




}
