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

    public static string libraryDataPath = Path.Combine(Directory.GetCurrentDirectory(), "YTlibrary.json");

    public class AudioHandler
    {

        public AudioHandler() { }

        public YTMusicSharp GetYTClient()
        {
            return YTClient;
        }
        public async Task<string> GetAudioData(string id) => await YTClient.GetYTAudioById(id);

    }



    public static void RegisterHandle(BrowserWindow win, string eventName, Delegate callback)
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



    public static async Task OpenLogWin(BrowserWindow win)
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
        RegisterHandle(win, "openLog", async () =>
        {
            await OpenLogWin(win);
        });

        RegisterHandle(win, "loginYT", () =>
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonPath = Path.Join(baseDirectory, "ytheaders.json");
            if (File.Exists(jsonPath))
            {

                JsonObject headers = (JsonObject)JsonNode.Parse(File.ReadAllText(jsonPath));

                YTClient = new YTMusicSharp(
                    youtubHeaders: headers,
                    workspacePath: baseDirectory
                );
                return true;
            }
            else
            {
                return false;
            }

        });

        RegisterHandle(win, "getLogInfo", async () =>
        {

            JsonObject loggedUser = await YTClient.AccountEndpoint.GetLoggedUser();

            return JsonSerializer.Serialize(loggedUser);

        });

        RegisterHandle(win, "LogOff", async () =>
        {
            File.Delete("./ytheaders.json");
        });

        //YOUTUBE HANDLERS

        RegisterLybraryHandlers(win);
        RegisterHomeHandlers(win);
        RegisterSearchHandlers(win);

        RegisterHandle(win, "GetFromDB", (string id, string filter) =>
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

        RegisterHandle(win, "subscribeArtist", async (string id, bool state) =>
        {
            await YTClient.InteractionsEndpoint.SetArtistSubscription(id, state);
        });

        RegisterHandle(win, "setSaveAlbum", (string browseId, bool state) =>
        {

            YTClient.InteractionsEndpoint.SetPlaylistSave(browseId, state);

        });

        RegisterHandle(win, "setVideoLike", async (string id, string likeStatus) =>
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

            await YTClient.InteractionsEndpoint.SetSongLikeStatus(id, likeStatusFinal);

        });

        RegisterHandle(win, "getSearchSugg", async (string key) =>
        {

            JsonArray sugesstions = await YTClient.SearchEndpoint.GetSearchSugg(key);

            return JsonSerializer.Serialize(sugesstions);

        });

        RegisterHandle(win, "getPageData", async (string type, string browseId) =>
        {

            string Return = "";

            switch (type)
            {
                case "album":
                    Return = JsonSerializer.Serialize(await YTClient.BrowseEndpoint.FetchAlbumDataSongsOnly(browseId));
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
        RegisterHandle(win, "addToPlaylistMenu", async () =>
        {
            return JsonSerializer.Serialize(await YTClient.InteractionsEndpoint.GetAddToPlaylistMenu());
        });

        RegisterHandle(win, "removeFromplaylist", async (string id, string setVideoId, string playlistId) =>
        {
            await YTClient.InteractionsEndpoint.RemoveVideoFromPlaylist(id, setVideoId, playlistId);
        });

        RegisterHandle(win, "addToplaylist", async (string[] ids, string playlistId) =>
        {
            await YTClient.InteractionsEndpoint.AddVideoToPlaylist(ids, playlistId);
        });

        //PLAYLISTS -------------------------------
        RegisterHandle(win, "EditPlaylistInfo", async (string playlistId, string name, string desc, string privacy) =>
        {

            PrivacyStatus PS = PrivacyStatus.UNLISTED;

            switch (privacy)
            {
                case "UNLISTED":
                    PS = PrivacyStatus.UNLISTED;
                    break;

                case "PRIVATE":
                    PS = PrivacyStatus.PRIVATE;
                    break;

                case "PUBLIC":
                    PS = PrivacyStatus.PUBLIC;
                    break;
            }

            await YTClient.InteractionsEndpoint.EditPLaylist(playlistId, pTitle: name, pDescriprtion: desc, privacyStatus: PS);

        });

        RegisterHandle(win, "CreatePlaylistInfo", async (string name, string desc, string privacy) =>
        {

            PrivacyStatus PS = PrivacyStatus.UNLISTED;

            switch (privacy)
            {
                case "UNLISTED":
                    PS = PrivacyStatus.UNLISTED;
                    break;

                case "PRIVATE":
                    PS = PrivacyStatus.PRIVATE;
                    break;

                case "PUBLIC":
                    PS = PrivacyStatus.PUBLIC;
                    break;
            }

            await YTClient.InteractionsEndpoint.CreatePlaylist(pTitle: name, pDescriprtion: desc, privacyStatus: PS);

        });

        RegisterHandle(win, "DeletePlaylist", async (string playlistId) =>
        {
            await YTClient.InteractionsEndpoint.DeletePLaylist(playlistId);
        });


    }

    public static async void RegisterEvents(BrowserWindow win)
    {

        string shortcut = "CommandOrControl+Shift+M";

        Electron.GlobalShortcut.Register(shortcut, async () =>
        {
            Electron.IpcMain.Send(win, "showWin");
        });

        win.OnClose += ()  =>
        {
            YTClient.ReleaseCached();
        };


    }





    public static void RegisterSearchHandlers(BrowserWindow win)
    {
        RegisterHandle(win, "search", async (string searckKey, string type) =>
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
        RegisterHandle(win, "getHome", async () =>
        {
            return JsonSerializer.Serialize(YTClient.BrowseEndpoint.FetchHomeSections());
        });
    }
    public static void RegisterLybraryHandlers(BrowserWindow win)
    {
        //LIBRARY PAGE
        RegisterHandle(win, "getLibraryPage", async () =>
        {

            string serializedData = JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryLandingPage());

            File.WriteAllText(libraryDataPath, serializedData);

            return serializedData;
        });

        RegisterHandle(win, "getSavedYTLibrary", async () =>
        {
            if (File.Exists(libraryDataPath))
            {
                return File.ReadAllText(libraryDataPath);
            }

            return "[]";
        });

        RegisterHandle(win, "getLibraryPlaylists", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Playlists));
        });

        RegisterHandle(win, "getLibraryAlbums", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Albums));
        });

        RegisterHandle(win, "getLibraryArtists", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Artists));
        });

        RegisterHandle(win, "getLibrarySubscribed", async () =>
        {
            return JsonSerializer.Serialize(YTClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Subscribed));
        });

    }


}
