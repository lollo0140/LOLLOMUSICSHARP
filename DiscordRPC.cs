using System.Text.Json.Nodes;
using DiscordRPC;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace LOLLOMUSICX
{
    public class DiscordRPCHandler
    {
        public const string DISCORD_APP_ID = "1242579109930864721";
        public static DiscordRpcClient client;

        public static void InitializeClient()
        {
            if (client == null)
            {
                client = new DiscordRpcClient(DISCORD_APP_ID);

                // Registra l'evento OnReady prima di inizializzare
                client.OnReady += (sender, e) =>
                {
                    SetRPCStatus();
                };
            }


            if (!client.IsInitialized)
            {
                client.Initialize();
            }
        }


        public static void SetRPCStatus()
        {
            if (client == null || !client.IsInitialized) return;

            client.SetPresence(new RichPresence()
            {
                Details = "Exploring homepage",
                State = "Idle",
                Assets = new Assets()
                {
                    LargeImageKey = "applabeldiscord",
                },
                Buttons = new Button[]
                {
                    new Button()
                    {
                        Label = "lollo0140/LOLLOMUSICSHARP",
                        Url = "https://github.com/lollo0140/LOLLOMUSICSHARP"
                    }
                }
            });
        }

        // Metodo consigliato per liberare le risorse alla chiusura della tua app
        public static void Deinitialize()
        {
            if (client != null && !client.IsDisposed)
            {
                client.Dispose();
                client = null;
            }
        }
    }
}
