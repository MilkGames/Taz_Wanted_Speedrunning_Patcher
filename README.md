Custom launcher and game patcher for **Taz: Wanted**.

**This patcher is the only one authorized for speedrun PC version of Taz: Wanted on [speedrun.com](https://www.speedrun.com/taz_wanted)!**

The source code and images were taken from **[Taz: Wanted Trainer and Patcher](https://github.com/MuxaJlbl4/Taz_Wanted_trainer_and_patcher)** project created by **[MuxaJlbl4 (MuLLlaH9!)](https://github.com/MuxaJlbl4)** with his permission.

These repositories and the licenses that come with them are also used in this project:
- [d3d8to9](https://github.com/crosire/d3d8to9)
- [ExtractTarGz](https://gist.github.com/ForeverZer0/a2cd292bd2f3b5e114956c00bb6e872b)
- [FormSerialisor](https://github.com/Skkay/FormSerialisor)
- [GlobalKeyboardHook](https://github.com/jparnell8839/globalKeyboardHook)

This patcher supports restoring game files after using Taz: Wanted Trainer and Patcher, provided that Taz.exe.backup in the root of the game folder is the original executable file dated 8/12/2002 for the US version of the game.

Also this patcher doesn't conflict with Taz: Wanted Trainer and Patcher, as it creates its own SpeedrunningPatcher.xml save file (instead of Patcher.xml).

## ✏ Requirements

- **Taz: Wanted PC Game** - Taz: Wanted US version
- **[Microsoft .NET Framework 4.8](https://go.microsoft.com/fwlink/?linkid=2088631) or newer** - Already preinstalled on [Windows 10 (version 1903) and newer](https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies#net-framework-48)
- **Internet Access (Optional)** - for downloading [d3d8to9](https://github.com/crosire/d3d8to9) wrapper and [ReadMe](https://milkgames.github.io/Taz_Wanted_Speedrunning_Patcher) view
- **[DirectX® End-User Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=35) (Optional)** - for some custom graphics wrappers

## 🔩 Patches
- Select patches and click **Patch & Play**
- **Patch game** button just patches files without starting the game
- To reset any patches click **Restore patches**

### ⌛ Launch options
- **Language:** - Change game language:
	- **English**
	- **French**
	- **German**
	- **Italian**
	- **Spanish**
- **NoCD Patch** - Remove CD check during startup
- **Skip Warning Banner** - Disable unskippable intro warning screen
- **Skip Logo Videos** - Disable Blitz, Infogrames and WB intro logos
- **Key Layout** - Assign Map and Pause actions to Back and Start controller buttons

### 📺 Video options
- **API:** - Download and replace graphics API to custom wrapper:
	- **D3d8 · vanilla** - Default DirectX 8.1
	- **D3d9 · [d3d8to9](https://github.com/crosire/d3d8to9)** - DirectX 9 wrapper
- **Res:** - Video resolution, prefilled with system defaults
- **Aspect:** - Aspect ratio **ONLY 4:3 AND 16:9!!!**
- **Windowed Mode** - Windowed mode with selected resolution and ratio
- **Voodoo Compatibility** - Simplified graphics mode, incompatible with windowed mode
- **Point Texture Filtering** - Nearest-Neighbour filtering for all textures (magnified textures only)
- **!NEW!** **Cartoon Lightning** - You can enable/disable Cartoon Lightning inside the patcher, without using the native launcher.
- **!NEW!** **Cartoon Outlines** - You can enable/disable Cartoon Outlines inside the patcher, without using the native launcher.
- **!NEW!** **Draw Distance** - Adjust Draw Distance inside the patcher, without using the native launcher.

## ⚙ Settings

### 🗺 Game path
You can select the game path manually by **Browse** and **Apply** buttons; It changes Windows registry value with Taz path

### 🛠 Trainer/Patcher options
- **Auto save config on exit** - Auto save app settings to SpeedrunningPatcher.xml file on exit
- **Save app config** - Save app settings to SpeedrunningPatcher.xml file
- **Reset app config** - Delete SpeedrunningPatcher.xml file and restart app with default settings
- **Update d3d8to9 API** - Download latest graphics API wrapper to the game folder ([d3d8to9](https://github.com/crosire/d3d8to9))
- **Delete saves (.sav)** - Delete game saves
- **Kill Taz.exe (Alt+F4)** - Terminate game process (hooked as Alt+F4 in the game)

### ⛳ Shortcuts
- **Play** - Start game
- **Launcher** - Open native Taz: Wanted launcher
- **Video** - Open native video setup
- **Audio** - Open native audio setup
- **Controls** - Open native controls setup
- **Explorer** - Open game folder in Windows explorer
- **GitHub** - Link to [Taz: Wanted Speedrunning Patcher](https://github.com/MilkGames/Taz_Wanted_Speedrunning_Patcher) repository

## 💡 Known Issues
- On some systems, after patching the game, Windows Defender removes Taz.exe and marks it as a trojan. (Solved, but on really rare occasions it may show up.)

## ➕ Additional information
- Original source code, images and README.md: **[Taz: Wanted Trainer and Patcher](https://github.com/MuxaJlbl4/Taz_Wanted_trainer_and_patcher)** project created by **[MuxaJlbl4 (MuLLlaH9!)](https://github.com/MuxaJlbl4)**
- Removing trainer features, adding allowed features for speedrun.com: **[Milk](https://www.youtube.com/channel/UC8ZrxS78M9TqB_2cMlIWJMA)[Games](https://github.com/MilkGames)**
- Testers: **[MuxaJlbl4 (MuLLlaH9!)](https://github.com/MuxaJlbl4)**, **[Cyclone](https://www.youtube.com/c/CycloneFN)[FN](https://pastebin.com/u/CycloneFN)**

The project was made at the request of CycloneFN for [Taz: Wanted Speedrunning Community](https://discord.gg/YJAVRbB8PK) in August 2023.
