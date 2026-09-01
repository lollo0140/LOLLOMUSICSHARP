using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using LOLLOMUSICX;
using Newtonsoft.Json.Linq;
using YoutubeMusic;

class IpcMain
{




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

                File.WriteAllText(Program.userSessionJSON, JsonSerializer.Serialize(cookiesJsonObject));


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
        Electron.IpcMain.On("setWinState", async (state) =>
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

            if (State == "minimize") win.Minimize();

            if (State == "maximize")
            {
                if (await win.IsMaximizedAsync()) win.Unmaximize();
                else win.Maximize();
            }

            if (State == "exit") win.Close();


        });




        //log window
        RegisterHandle(win, "openLog", async () =>
        {
            await OpenLogWin(win);
        });

        RegisterHandle(win, "loginYT", () =>
        {
            if (File.Exists(Program.userSessionJSON))
            {
                JsonObject headers = (JsonObject)JsonNode.Parse(File.ReadAllText(Program.userSessionJSON));

                Program.yTMusicClient = new(headers);
                return true;
            }
            else
            {
                return false;
            }

        });

        RegisterHandle(win, "getLogInfo", async () =>
        {

            JsonObject loggedUser = await Program.yTMusicClient.AccountEndpoint.GetLoggedUser();

            return JsonSerializer.Serialize(loggedUser);

        });

        RegisterHandle(win, "LogOff", async () =>
        {
            File.Delete(Program.userSessionJSON);
            Program.yTMusicClient = new();
        });

        //YOUTUBE HANDLERS

        RegisterLybraryHandlers(win);
        RegisterHomeHandlers(win);
        RegisterSearchHandlers(win);
        RegisterSystemHandlers(win);


        RegisterHandle(win, "subscribeArtist", async (string id, bool state) =>
        {
            await Program.yTMusicClient.InteractionsEndpoint.SetArtistSubscription(id, state);
        });

        RegisterHandle(win, "setSaveAlbum", (string browseId, bool state) =>
        {

            Program.yTMusicClient.InteractionsEndpoint.SetPlaylistSave(browseId, state);

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

            await Program.yTMusicClient.InteractionsEndpoint.SetSongLikeStatus(id, likeStatusFinal);

        });

        RegisterHandle(win, "getSearchSugg", async (string key) =>
        {

            JsonArray sugesstions = await Program.yTMusicClient.SearchEndpoint.GetSearchSugg(key);

            return JsonSerializer.Serialize(sugesstions);

        });

        RegisterHandle(win, "getPageData", async (string type, string browseId) =>
        {

            string Return = "";

            switch (type)
            {
                case "album":
                    Return = JsonSerializer.Serialize(await Program.yTMusicClient.BrowseEndpoint.FetchAlbumData(browseId));
                    break;

                case "playlist":
                    Return = JsonSerializer.Serialize(await Program.yTMusicClient.BrowseEndpoint.FetchPlaylistData(browseId));
                    break;

                case "artist":
                    Return = JsonSerializer.Serialize(await Program.yTMusicClient.BrowseEndpoint.FetchArtistPage(browseId));
                    break;

                default:
                    break;
            }


            return Return;

        });


        //add to playlist
        RegisterHandle(win, "addToPlaylistMenu", async () =>
        {
            return JsonSerializer.Serialize(await Program.yTMusicClient.InteractionsEndpoint.GetAddToPlaylistMenu());
        });

        RegisterHandle(win, "removeFromplaylist", async (string id, string setVideoId, string playlistId) =>
        {
            await Program.yTMusicClient.InteractionsEndpoint.RemoveVideoFromPlaylist(id, setVideoId, playlistId);
        });

        RegisterHandle(win, "addToplaylist", async (string[] ids, string playlistId) =>
        {
            await Program.yTMusicClient.InteractionsEndpoint.AddVideoToPlaylist(ids, playlistId);
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

            await Program.yTMusicClient.InteractionsEndpoint.EditPLaylist(playlistId, pTitle: name, pDescriprtion: desc, privacyStatus: PS);

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

            await Program.yTMusicClient.InteractionsEndpoint.CreatePlaylist(pTitle: name, pDescriprtion: desc, privacyStatus: PS);

        });

        RegisterHandle(win, "DeletePlaylist", async (string playlistId) =>
        {
            await Program.yTMusicClient.InteractionsEndpoint.DeletePLaylist(playlistId);
        });


        //settings
        RegisterHandle(win, "getSettings", async () =>
        {

            string fileContent = File.ReadAllText(Program.settingsJSONPath);
            JsonNode settings = JsonNode.Parse(fileContent);

            return fileContent;

        });

        RegisterHandle(win, "saveSettings", async (string jsonString) =>
        {

            File.WriteAllText(Program.settingsJSONPath, jsonString);

        });



    }

    public static async Task SetWinHide(BrowserWindow win, bool value)
    {
        if (OperatingSystem.IsLinux())
        {
            if ((bool)value)
            {
                await win.WebContents.ExecuteJavaScriptAsync("document.body.style.opacity = '0';");
                win.SetIgnoreMouseEvents(true);
                win.SetSkipTaskbar(true);
            }
            else
            {
                await win.WebContents.ExecuteJavaScriptAsync("document.body.style.opacity = '1';");
                win.SetIgnoreMouseEvents(false);
                win.SetSkipTaskbar(false);
            }
        }
        else
        {
            if ((bool)value)
            {
                win.Hide();
            }
            else
            {
                win.Show();
            }
        }


    }

    public static async Task RegisterEvents(BrowserWindow win)
    {


        await Electron.IpcMain.On("setHideWinValue", async (value) =>
        {

            await SetWinHide(win, (bool)value);

        });


        win.OnClose += () =>
        {
            Utility.CleanDir(Program.cachedVideosPath);
        };
    }




    public static void RegisterSystemHandlers(BrowserWindow win)
    {
        RegisterHandle(win, "openDirPicker", async () =>
        {
            return await ElectronFunctions.OpenDirectoryPicker(win);
        });

        RegisterHandle(win, "downloadSong", async (string sedialized) =>
        {
            try
            {
                var songContent = (JsonObject)JsonNode.Parse(sedialized);
                System.Console.WriteLine($"DOWNLOADING VIDEO: {songContent["id"].GetValue<string>()}");

                if (!songContent.ContainsKey("id"))
                {
                    return false;
                }

                string songId = songContent["id"].GetValue<string>();

                string destinationPath = Path.Join(Program.downloadPAth, $"{songId}.webm");


                songContent["path"] = destinationPath;


                var _ = Path.Join(Program.cachedVideosPath, $"{songId}.webm");
                if (File.Exists(_))
                {
                    File.Copy(_, destinationPath);
                }
                else
                {
                    await Program.yTMusicClient.DownloadVideoById(songId, destinationPath);
                }




                JsonObject json = Utility.ReadJsonObject(Program.JSONDownloadsPath);
                json[songId] = songContent;

                Utility.WriteJsonFile(Program.JSONDownloadsPath, json);

                return true;

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error downloading video: " + e.ToString());
                return false;
            }
        });

        RegisterHandle(win, "removeLocal", async (string id) =>
        {
            var downloads = (JsonObject)JsonNode.Parse(File.ReadAllText(Program.JSONDownloadsPath));
            if (downloads.ContainsKey(id))
            {
                var p = downloads[id]["path"].GetValue<string>();

                downloads.Remove(id);
                File.Delete(p);
            }
            File.WriteAllText(Program.JSONDownloadsPath, downloads.ToString());
        });



        RegisterHandle(win, "scanDownloaded", async () =>
        {
            JsonObject _ = Utility.ReadJsonObject(Program.JSONDownloadsPath);

            JsonArray arr = [.. _.Select(x => x.Key).ToList()];

            return JsonSerializer.Serialize(arr);
        });

        RegisterHandle(win, "getDownloaded", async () =>
        {
            var _ = Utility.ReadJsonObject(Program.JSONDownloadsPath);

            JsonArray array = [.. _.Select(element => element.Value.DeepClone())];
            return JsonSerializer.Serialize(array);
        });

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
                return JsonSerializer.Serialize(Program.yTMusicClient.SearchEndpoint.GenericSearch(searckKey));
            }

            return JsonSerializer.Serialize(Program.yTMusicClient.SearchEndpoint.SpecificSearch(searckKey, contentType));
        });
    }
    public static void RegisterHomeHandlers(BrowserWindow win)
    {
        RegisterHandle(win, "getHome", async () =>
        {
            return JsonSerializer.Serialize(Program.yTMusicClient.BrowseEndpoint.FetchHomeSections());
        });
    }
    public static void RegisterLybraryHandlers(BrowserWindow win)
    {
        //LIBRARY PAGE
        RegisterHandle(win, "getLibraryPage", async () =>
        {

            string serializedData = JsonSerializer.Serialize(Program.yTMusicClient.LibraryEndpoint.GetLibraryLandingPage());

            return serializedData;
        });


        RegisterHandle(win, "getLibraryPlaylists", async () =>
        {
            return JsonSerializer.Serialize(Program.yTMusicClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Playlists));
        });

        RegisterHandle(win, "getLibraryAlbums", async () =>
        {
            return JsonSerializer.Serialize(Program.yTMusicClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Albums));
        });

        RegisterHandle(win, "getLibraryArtists", async () =>
        {
            return JsonSerializer.Serialize(Program.yTMusicClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Artists));
        });

        RegisterHandle(win, "getLibrarySubscribed", async () =>
        {
            return JsonSerializer.Serialize(Program.yTMusicClient.LibraryEndpoint.GetLibraryContent(ContentFilter.Subscribed));
        });

    }


}
