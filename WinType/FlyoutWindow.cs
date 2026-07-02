using System;
using System.IO;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;

class FlyoutWindow
{

    public static async void setWinStaticPosition(BrowserWindow win)
    {
        var display = await Electron.Screen.GetPrimaryDisplayAsync();

        var height = display.WorkArea.Height;
        var width = display.WorkArea.Width;

        int left = (width / 2) - 150 + 7;

        win.SetResizable(true);
        win.SetPosition(left, 0);
        win.SetSize(300, 54);
        win.SetResizable(false);

    }

    public static async void setWinOpenedPosition(BrowserWindow win)
    {
        var display = await Electron.Screen.GetPrimaryDisplayAsync();

        var height = display.WorkArea.Height;
        var width = display.WorkArea.Width;

        win.SetPosition(7, 0);
        win.SetResizable(true);
        win.SetSize(width, height);
        win.SetResizable(false);

    }



}
