# LOLLOMUSICSHARP (LOLLOMUSICX)

Desktop music client based on YouTube Music, developed with .NET 10 (ASP.NET Core), Electron.NET, and a modern frontend interface powered by Svelte 5, Tailwind CSS, and GSAP.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Key Features](#key-features)
- [Technical Architecture](#technical-architecture)
- [System Requirements](#system-requirements)
- [Installation Instructions](#installation-instructions)
- [Running in Development Mode](#running-in-development-mode)
- [Build and Packaging](#build-and-packaging)
- [Configuration](#configuration)
- [Keyboard Shortcuts](#keyboard-shortcuts)
- [Repository Structure](#repository-structure)
- [License](#license)

---

## Project Overview

LOLLOMUSICSHARP (internally identified as LOLLOMUSICX) is an open-source desktop music player for Windows, Linux, and macOS, designed to deliver a fast, responsive, and clutter-free YouTube Music experience without the overhead of standard web browsers.

The software pairs a high-performance backend built on .NET 10 and ASP.NET Core with the Electron.NET desktop runtime bridge. The user interface is crafted using Svelte 5 and Tailwind CSS, providing immediate reactivity, clean layouts, and smooth animations powered by GSAP.

The client features high-throughput audio streaming with HTTP range processing and local disk caching, an offline download manager, YouTube Music account synchronization, real-time Discord Rich Presence integration, global media hotkeys via SharpHook, and a compact floating pill player mode with global hotkey toggling.

---

## Key Features

- Optimized Audio Streaming: Seamless audio stream playback with range processing and intelligent local disk caching to minimize bandwidth and latency.
- Offline Download Manager: Download audio tracks to a customizable local folder with automated offline library indexing and discovery.
- YouTube Music Account Synchronization: Secure authentication with your YouTube Music account, library synchronization, track liking, and artist channel subscriptions.
- Complete Playlist Management: Create, edit, update metadata, and add or remove tracks directly inside the client interface.
- Discord Rich Presence: Real-time Discord status updates showing the active track, artist name, album art cover, and repository links.
- Dual Window Interfaces:
  - Standard Mode: Full-featured frameless window with custom titlebar controls, responsive navigation sidebar, and library explorer.
  - Pill Mode (Floating Mini-Player): Compact, transparent, always-on-top widget designed for unobtrusive listening.
- Global Media Keys: Full keyboard multimedia control (Play/Pause, Next, Previous) handled through SharpHook even when the app is minimized.
- System Tray Integration: Background persistence with a tray context menu to show, hide, or terminate the player process.

---

## Technical Architecture

- Backend:
  - Framework: .NET 10.0 (Microsoft.NET.Sdk.Web)
  - Desktop Engine: ElectronNET.API (v23.6.2)
  - Local Web Server: ASP.NET Core Kestrel (local REST endpoints for audio streaming and image caching)
  - Discord Integration: DiscordRichPresence (v1.6.1.70)
  - Global Keyboard Hooks: SharpHook (v7.1.3)
  - YouTube Music Integration: YTMusicSharp / YoutubeMusic

- Frontend:
  - Framework: Svelte 5 with SvelteKit (adapter-static)
  - Styling: Tailwind CSS v4
  - Motion: GSAP (GreenSock Animation Platform)
  - Bundler: Vite 8

---

## System Requirements

Before installing and compiling the application, verify that the following prerequisites are installed on your workstation:

- .NET 10.0 SDK or newer: Verify using `dotnet --version`
- Node.js (v18.x LTS or v20.x or higher) and npm: Verify using `node -v` and `npm -v`
- Electron.NET CLI: Global command line tool required for electronize commands:
  ```bash
  dotnet tool install -g ElectronNET.CLI
  ```
- Git: For repository cloning and dependency management

Note: The project references a local library project `..\YTMusicSharp\YTMusicSharp.csproj`. Ensure the referenced folder is placed in the sibling directory if building from source.

---

## Installation Instructions

### 1. Clone the Repository

Clone the project repository to your machine:

```bash
git clone https://github.com/lollo0140/LOLLOMUSICSHARP.git
cd LOLLOMUSICSHARP
```

### 2. Install Frontend Dependencies

Navigate into the `ClientApp` directory and install the required npm packages:

```bash
cd ClientApp
npm install
```

### 3. Build Frontend Static Assets

Compile the SvelteKit frontend into static distribution files for `wwwroot`:

```bash
npm run build
cd ..
```

### 4. Restore .NET Packages

From the repository root folder, restore all NuGet dependencies:

```bash
dotnet restore
```

---

## Running in Development Mode

Two development workflows are supported:

### Method A: Full Desktop Launch with Electron.NET

Run the Electron.NET launch command from the repository root:

```bash
electronize start
```

This command will:
1. Boot the ASP.NET Core backend server on the assigned port.
2. Initialize and open the Electron desktop container window.
3. Attach the secure IPC bridge and preload script.

### Method B: Frontend Live Reload

If you are developing the user interface in Svelte:

1. Terminal 1 (in the `ClientApp` directory):
   ```bash
   npm run dev
   ```
   The Vite development server will start at `http://localhost:5173`.

2. Terminal 2 (in the repository root):
   ```bash
   electronize start
   ```
   (In `Program.cs`, the window will load `http://localhost:5173/` during development).

---

## Build and Packaging

To produce standalone desktop binaries:

### Windows (x64)

```bash
electronize build /target win
```

### Linux

```bash
electronize build /target linux
```

### macOS (Experimental / To be tested)

```bash
electronize build /target osx
```

Note on macOS: macOS packaging and execution is supported through Electron.NET, but is currently experimental and yet to be thoroughly tested on physical Apple hardware.

Compiled binaries will be generated inside `bin/Desktop` according to the settings in `electron.manifest.json`.

---

## Configuration

The application automatically creates and manages configuration and state files in the executable root folder:

### `settings.json`
Contains user preferences for window appearance and download paths:

```json
{
  "appearence": {
    "winStyle": "float"
  },
  "localData": {
    "downloadPath": "none"
  }
}
```

- `winStyle`: Set to `"float"` for the standard resizable window, or `"pill"` for the compact transparent mini-player.
- `downloadPath`: Directory path for offline audio files (if set to `"none"`, `./download` is used).

### `session.json`
Stores YouTube Music authentication tokens and user session data.

### `downloaded.json`
Maintains an index of downloaded tracks and metadata for offline listening.

---

## Keyboard Shortcuts

- `Ctrl + Shift + M` (Windows / Linux) or `Cmd + Shift + M` (macOS):
  Toggles the floating Pill mini-player widget.
- Hardware media keys (Play, Pause, Next Track, Previous Track):
  Handled globally across the operating system via SharpHook.

---

## Repository Structure

```text
LOLLOMUSICSHARP/
|-- ClientApp/                   Frontend source code in Svelte 5 and Tailwind CSS
|   |-- src/                     SvelteKit components, routes, and state
|   |-- package.json             Frontend npm dependencies
|   +-- vite.config.js           Vite bundler configuration
|-- wwwroot/                     Static web assets served by Kestrel
|   |-- assets/                  Icons, media controls, and loading animations
|   +-- index.html               Application HTML entry point
|-- ApiMaps.cs                   Local REST endpoints (/api/audio, /api/img)
|-- DiscordRPC.cs                Discord Rich Presence provider
|-- IpcMain.cs                   IPC communication between Electron and C#
|-- Program.cs                   ASP.NET Core host and Electron window lifecycle
|-- Utility.cs                   Filesystem utilities and JSON helpers
|-- LOLLOMUSICX.csproj           .NET 10 project specification
|-- electron.manifest.json       Electron.NET packaging manifest
|-- preload.js                   Secure Electron context isolation bridge
+-- README.md                    Repository documentation
```

---

## License

This project is released under terms established by the author (`lollo0140`). Please refer to the repository for details on usage rights and distribution.
