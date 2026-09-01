using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ElectronNET.API;
using ElectronNET.API.Entities;
using YoutubeMusic;
using System.Drawing;
using LOLLOMUSICX;

class Program
{
    public static YTMusicSharp yTMusicClient;
    public static string userSessionJSON = Path.Combine(AppContext.BaseDirectory, "session.json");
    public static string cachedVideosPath = Path.Combine(AppContext.BaseDirectory, "cached");
    public static string settingsJSONPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
    public static string downloadPAth = Path.Combine(AppContext.BaseDirectory, "download");
    public static string JSONDownloadsPath = Path.Combine(AppContext.BaseDirectory, "downloaded.json");

    private static void PreAppOperations()
    {

        if (!Directory.Exists(cachedVideosPath)) Directory.CreateDirectory(cachedVideosPath);
        if (!Directory.Exists(downloadPAth)) Directory.CreateDirectory(downloadPAth);

        if (File.Exists(userSessionJSON))
            yTMusicClient = new((JsonObject)JsonNode.Parse(File.ReadAllText(userSessionJSON)));
        else
            yTMusicClient = new();

        CreateSettingsFile();

        if (File.Exists(settingsJSONPath))
        {
            JsonObject settings = (JsonObject)JsonNode.Parse(File.ReadAllText(settingsJSONPath));
            if (settings["localData"]?["downloadPath"]?.GetValue<string>() != "none")
                downloadPAth = settings["localData"]["downloadPath"].GetValue<string>();
        }

        Utility.ValidateDownloaded();
    }

    public static void Main(string[] args)
    {
        PreAppOperations();

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.WebHost.UseElectron(args);

        var app = builder.Build();

        app.MapGet("/api/audio/{id}", async (string id) =>
        {
            try
            {
                if (Utility.IsVideoLocal(id))
                {
                    string localPath = Utility.GetLocalStreamingPath(id);
                    return Results.File(localPath, contentType: "audio/webm", enableRangeProcessing: true);
                }

                string P = Path.Combine(cachedVideosPath, $"{id}.webm");

                if (!File.Exists(P))
                {
                    await yTMusicClient.DownloadVideoById(id, P);
                }

                return Results.File(P, contentType: "audio/webm", enableRangeProcessing: true);
            }
            catch
            {
                return Results.Problem($"Error while loading video: {id}");
            }
        });

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        if (HybridSupport.IsElectronActive)
        {
            Electron.App.CommandLine.AppendSwitch("enable-gpu-rasterization");
            Electron.App.CommandLine.AppendSwitch("enable-zero-copy");

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () => await CreateElectronWindowAsync(app));
            });
        }

        app.Run();
    }

    static void CreateTray(WebApplication app, BrowserWindow window)
    {
#if WINDOWS
        var menuItems = new MenuItem[]
        {
            new MenuItem { Label = "Mostra App", Click = () => window.Show() },
            new MenuItem { Label = "Nascondi", Click = () => window.Hide() },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Label = "Esci", Click = () => Electron.App.Quit() }
        };

        string iconPath = Path.Combine(AppContext.BaseDirectory, "Icon.png");
        if (File.Exists(iconPath))
        {
            Task.Run(async () =>
            {
                await Electron.Tray.Show(iconPath, menuItems);
                await Electron.Tray.SetToolTip("LOLLOMUSICX");
            });
        }
#endif
    }

    static void CreateSettingsFile()
    {
        if (!File.Exists(settingsJSONPath))
        {
            JsonObject settings = new()
            {
                ["appearence"] = new JsonObject
                {
                    ["winStyle"] = "float"
                },
                ["localData"] = new JsonObject
                {
                    ["downloadPath"] = "none"
                }
            };

            Utility.WriteJsonFile(settingsJSONPath, settings);
        }
    }

    static async Task CreateElectronWindowAsync(WebApplication app)
    {
        string preloadPath = Path.Combine(AppContext.BaseDirectory, "preload.js");
        if (!File.Exists(preloadPath))
        {
            preloadPath = Path.Combine(Directory.GetCurrentDirectory(), "preload.js");
        }

        var settings = Utility.ReadJsonObject(settingsJSONPath);

        bool isPill = settings["appearence"]["winStyle"].GetValue<string>().Equals("pill");

        var options = new BrowserWindowOptions
        {
            Title = "LOLLOMUSICX",
            Frame = false,
            AutoHideMenuBar = true,
            Show = true,
            Transparent = isPill,
            BackgroundColor = isPill ? "#00000000" : "black",
            Resizable = !isPill,
            Movable = true,
            MinHeight = isPill ? 0 : 750,
            MinWidth = isPill ? 0 : 1400,
            SkipTaskbar = !isPill,
            AlwaysOnTop = isPill,
            Icon = Path.Combine(AppContext.BaseDirectory, "wwwroot", "Icon.png"),
            WebPreferences = new WebPreferences
            {
                ContextIsolation = true,
                NodeIntegration = false,
                BackgroundThrottling = false,
                Offscreen = false,
                Preload = preloadPath
            }
        };

        var window = await Electron.WindowManager.CreateWindowAsync(options);

        _ = IpcMain.RegisterEvents(window);
        IpcMain.RegisterHandlers(window);

        window.LoadURL($"http://localhost:{BridgeSettings.WebPort}/");

        // DEBUG ONLY, comment this line in release
        //window.LoadURL("http://localhost:5173/");

        string shortcut = "CommandOrControl+Shift+M";

        Electron.GlobalShortcut.Register(shortcut, async () =>
        {
            if (!isPill) return;

            System.Console.WriteLine("Shortcut attivato!");

            await IpcMain.SetWinHide(window, false);

            Electron.IpcMain.Send(window, "showWin");
        });

        CreateTray(app, window);

        if (isPill) FlyoutWindow.setWinOpenedPosition(window);
    }
}
