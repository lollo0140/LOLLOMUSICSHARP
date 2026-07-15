using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;
using YoutubeMusic;

class IpcMain
{

    public static YTMusicSharp YTClient;

    public static void registerHandle(BrowserWindow win, string eventName, Delegate callback)
    {

        Electron.IpcMain.On(eventName, async (data) =>
        {
            try
            {
                var jData = data as JObject;
                if (jData == null) return;

                string requestId = jData["requestId"]?.ToString();
                var payload = jData["payload"] as JArray;

                var methodParameters = callback.Method.GetParameters();
                object[] convertedArguments = new object[methodParameters.Length];

                for (int i = 0; i < methodParameters.Length; i++)
                {
                    if (payload != null && i < payload.Count)
                    {
                        convertedArguments[i] = payload[i].ToObject(methodParameters[i].ParameterType);
                    }
                    else
                    {
                        convertedArguments[i] = methodParameters[i].ParameterType.IsValueType
                            ? Activator.CreateInstance(methodParameters[i].ParameterType)
                            : null;
                    }
                }
                object result = callback.DynamicInvoke(convertedArguments);

                if (result is Task task)
                {
                    await task;
                    var resultProperty = task.GetType().GetProperty("Result");
                    if (resultProperty != null)
                    {
                        result = resultProperty.GetValue(task);
                    }
                    else
                    {
                        result = null;
                    }
                }

                Electron.IpcMain.Send(win, $"{eventName}-reply-{requestId}", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante l'esecuzione di {eventName}: {ex.Message}");
                // Opzionale: puoi inviare un messaggio di errore al frontend se lo desideri
            }
        });
    }



    public static async Task openLogWin(BrowserWindow win)
    {
        var tcs = new TaskCompletionSource<bool>();

        var options = new BrowserWindowOptions
        {
            Show = false,
            WebPreferences = new WebPreferences
            {
                ContextIsolation = true,
                NodeIntegration = false,
                Partition = "temp_youtube_session"
            }
        };

        var YTWindow = await Electron.WindowManager.CreateWindowAsync(options);


        YTWindow.OnReadyToShow += () =>
        {
            win.Hide();
            YTWindow.Show();
        };

        YTWindow.LoadURL("https://accounts.google.com/ServiceLogin?service=youtube&continue=https://music.youtube.com/");

        YTWindow.OnPageTitleUpdated += async (e) =>
        {

            if (e.Contains("YouTube Music"))
            {
                //Thread.Sleep(1000);


                var filter = new CookieFilter
                {
                    Url = "https://music.youtube.com"
                };


                var cookies = await YTWindow.WebContents.Session.Cookies.GetAsync(filter);

                JsonObject cookiesJsonObject = new JsonObject();
                foreach (var cookie in cookies)
                {

                    cookiesJsonObject[cookie.Name] = cookie.Value;

                }

                File.WriteAllText("./ytheaders.json", JsonSerializer.Serialize(cookiesJsonObject));


                YTWindow.Destroy();
                Electron.IpcMain.Send(win, "reloadLogInfo");
                win.Show();
                tcs.TrySetResult(true);
            }

        };

        YTWindow.OnClosed += () =>
        {
            win.Show();
            tcs.TrySetResult(false);
        };

        await tcs.Task;
    }




    public static void RegisterHandlers(BrowserWindow win)
    {
        Electron.IpcMain.On("setWinState", (state) =>
        {

            string State = (string)state;

            if (State == "close")
            {
                FlyoutWindow.setWinStaticPosition(win);
            }

            if (State == "open")
            {
                FlyoutWindow.setWinOpenedPosition(win);
            }

        });




        //log window
        registerHandle(win, "openLog", async () =>
        {
            await openLogWin(win);
        });

        registerHandle(win, "loginYT", () =>
        {
            if (File.Exists("./ytheaders.json"))
            {
                JsonObject headers = (JsonObject)JsonNode.Parse(File.ReadAllText("./ytheaders.json"));

                YTClient = new YTMusicSharp(
                    youtubHeaders: headers,
                    workspacePath: "./"
                );
                return true;
            }
            else
            {
                return false;
            }

        });

        registerHandle(win, "getLogInfo", async () =>
        {

            JsonObject loggedUser = await YTClient.AccountEndpoint.GetLoggedUser();

            return JsonSerializer.Serialize(loggedUser);

        });

        registerHandle(win, "LogOff", async () =>
        {
            File.Delete("./ytheaders.json");
        });

        //YOUTUBE HANDLERS

        RegisterLybraryHandlers(win);
        RegisterHomeHandlers(win);
        RegisterSearchHandlers(win);

        registerHandle(win, "GetFromDB", (string id, string filter) =>
        {

            DB_filter F;


            switch (filter)
            {
                case "album":
                    F = DB_filter.ALBUM;
                    break;

                case "playlist":
                    F = DB_filter.PLAYLIST;
                    break;

                case "artist":
                    F = DB_filter.ARTIST;
                    break;

                case "library":
                    F = DB_filter.LIBRARY;
                    break;

                case "cached":
                    F = DB_filter.CACHEDSONG;
                    break;

                case "downloaded":
                    F = DB_filter.DOWNLOADED;
                    break;

                default:
                    return "none";
            }

            return JsonSerializer.Serialize(YTClient.GetFromLocalDB(F, id));
        });

        registerHandle(win, "subscribeArtist", async (string id, bool state) =>
        {
            YTClient.InteractionsEndpoint.SetArtistSubscription(id, state);
        });

        registerHandle(win, "setSaveAlbum", (string browseId, bool state) =>
        {

            YTClient.InteractionsEndpoint.SetPlaylistSave(browseId, state);

        });

        registerHandle(win, "setVideoLike", async (string id, string likeStatus) =>
        {

            LikeStatus likeStatusFinal = LikeStatus.NEUTRAL;

            switch (likeStatus)
            {
                case "LIKE":
                    likeStatusFinal = LikeStatus.LIKE;
                    break;
                case "DISLIKE":
                    likeStatusFinal = LikeStatus.DISLIKE;
                    break;
                case "NEUTRAL":
                    likeStatusFinal = LikeStatus.NEUTRAL;
                    break;
            }

            YTClient.InteractionsEndpoint.SetSongLikeStatus(id, likeStatusFinal);

        });

        registerHandle(win, "getSearchSugg", async (string key) =>
        {

            JsonArray sugesstions = await YTClient.SearchEndpoint.GetSearchSugg(key);

            return JsonSerializer.Serialize(sugesstions);

        });

        registerHandle(win, "getPageData", async (string type, string browseId) =>
        {

            string Return = "";

            switch (type)
            {
                case "album":
                    Return = JsonSerializer.Serialize(await YTClient.BrowseEndpoint.FetchAlbumData(browseId));
                    break;

                case "playlist":
                    Return = JsonSerializer.Serialize(await YTClient.BrowseEndpoint.FetchPlaylistData(browseId));
                    break;

                case "artist":
                    Return = JsonSerializer.Serialize(await YTClient.BrowseEndpoint.FetchArtistPage(browseId));
                    break;

                default:
                    break;
            }


            return Return;

        });


        //add to playlist
        registerHandle(win, "addToPlaylistMenu", async () =>
        {
           return JsonSerializer.Serialize(await YTClient.InteractionsEndpoint.GetAddToPlaylistMenu());
        });

        registerHandle(win, "addToplaylist", async (string[] ids, string playlistId) =>
        {
            YTClient.InteractionsEndpoint.AddVideoToPlaylist(ids, playlistId);
        });

    }

    public static async void RegisterEvents(BrowserWindow win)
    {

        string shortcut = "CommandOrControl+Shift+M";

        Electron.GlobalShortcut.Register(shortcut, async () =>
        {
            Electron.IpcMain.Send(win, "showWin");
        });




    }





    public static void RegisterSearchHandlers(BrowserWindow win)
    {
        registerHandle(win, "search", async (string searckKey, string type) =>
        {
            ContentType contentType;

            switch (type)
            {
                case "all":
                    contentType = ContentType.All;
                    break;

                case "traks":
                    contentType = ContentType.Track;
                    break;

                case "albums":
                    contentType = ContentType.Album;
                    break;

                case "artists":
                    contentType = ContentType.Artist;
                    break;

                case "playlists":
                    contentType = ContentType.Playlist;
                    break;

                case "videos":
                    contentType = ContentType.Video;
                    break;

                default:
                    contentType = ContentType.All;
                    break;
            }


            if (contentType == ContentType.All)
            {
                return JsonSerializer.Serialize(YTClient.SearchEndpoint.GenericSearch(searckKey));
            }

            return JsonSerializer.Serialize(YTClient.SearchEndpoint.SpecificSearch(searckKey, contentType));
        });
    }
    public static void RegisterHomeHandlers(BrowserWindow win)
    {
        registerHandle(win, "getHome", async () =>
        {
            return JsonSerializer.Serialize(YTClient.BrowseEndpoint.FetchHomeSections());
        });
    }
    public static void RegisterLybraryHandlers(BrowserWindow win)
    {
        //LIBRARY PAGE
        registerHandle(win, "getLibraryPage", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryLandingPage());
        });

        registerHandle(win, "getLibraryPlaylists", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Playlists));
        });

        registerHandle(win, "getLibraryAlbums", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Albums));
        });

        registerHandle(win, "getLibraryArtists", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Artists));
        });

        registerHandle(win, "getLibrarySubscribed", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Subscribed));
        });

    }


}
