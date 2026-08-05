using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;

class ElectronFunctions
{
    public static async Task<string> OpenDirectoryPicker(BrowserWindow activeWin)
    {

        activeWin.SetAlwaysOnTop(false);

        var options = new OpenDialogOptions
        {
            Title = "Select a directory",
            Properties = new[]
            {
                OpenDialogProperty.openDirectory
            }
        };

        string[] result = await Electron.Dialog.ShowOpenDialogAsync(activeWin, options);

        if (result != null && result.Length > 0)
        {
            string cartellaSelezionata = result.First();
            activeWin.SetAlwaysOnTop(true);
            return cartellaSelezionata;
        }

        activeWin.SetAlwaysOnTop(true);
        return null;
    }
}
