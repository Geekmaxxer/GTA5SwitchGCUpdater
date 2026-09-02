# GTA 5 Switch Gameconfig.xml Updater

A Windows desktop application for updating `gameconfig.xml` in GTA V Switch `update.rpf` files without using OpenIV or CodeWalker directly.

## What it does

- Allows users to select their own `gameconfig.xml` file
- Opens the `update.rpf` file from their GTA V Switch installation
- Replaces the existing `gameconfig.xml` inside the RPF with the user's version
- Saves the updated RPF to a user-selected location

## Requirements

To **run** the published exe:
- Windows 10/11 x64 (nothing else — the build is self-contained and portable)

To **build** it yourself:
- Windows 10/11 x64
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

## Usage

1. Launch `GTASwitchGCUpdater.exe`
2. Click **Browse** next to "1. Select gameconfig.xml" and choose your gameconfig.xml file
3. Click **Browse** next to "2. Select update.rpf file" and choose your GTA V Switch `update.rpf`
4. Click **3. Update RPF**
5. Choose a location to save the updated `update.rpf` file
6. Done! Replace your original `update.rpf` with the newly generated file
- If you want to edit your GTA radio, the same method applies but with different .rpf files and your own music files. -> [gta-radio-editor](https://github.com/Geekmaxxer/gta-radio-editor)

## Build from source

Clone with submodules to include CodeWalker Core:

```powershell
git clone --recurse-submodules https://github.com/Geekmaxxer/GTA5SwitchGCUpdater
cd gta-gameconfig-updater\GTASwitchGCUpdater
dotnet publish -c Release
```

The project file already sets `RuntimeIdentifier`, `SelfContained`, and `PublishSingleFile`,
so no extra command-line flags are needed — the command above is enough to produce a fully
portable, single-file `.exe`.

The build will be output to:

```
GTA5SwitchGCUpdater\bin\Release\net8.0-windows\win-x64\
```
or for the portable .exe
```
GTA5SwitchGCUpdater\bin\Release\net8.0-windows\win-x64\publish
```

`GTA5SwitchGCUpdater.exe` in that folder is the finished, portable app: it's a single file
with no dependent DLLs, requires no .NET runtime installed on the target machine, and can be
copied to and run from anywhere (a USB drive, another PC, etc.).

> Note: because it's self-contained, the exe bundles its own copy of the .NET runtime, so
> expect it to be roughly 60–100 MB rather than a few hundred KB.

## Update checking

On startup, the app silently checks
[`/releases/latest`](https://github.com/Geekmaxxer/GTA5SwitchGCUpdater/releases/latest)
on GitHub and compares its tag to the running app's version (shown in small
text in the status bar, and in the title bar). That endpoint always
resolves to the newest published, non-draft, non-prerelease release, so a
beta build with a higher-looking version number - like the earlier
`v0.1-beta`/`v0.2-beta`/`v0.3-beta` tags - is never mistaken for the latest
stable release.

If a newer version is found, a small dialog offers **Take me there** (opens
the release page in your browser) or **I'm good** (dismisses it for the
rest of that session). There's no persisted "don't ask again" - closing and
reopening the app checks again.

If you're maintaining this repo: bump `AppVersion.Current` in
`AppVersion.cs` (and `<Version>` in the `.csproj`) every time you cut a new
version, and make sure it's published as an actual GitHub **Release** with
the "pre-release" checkbox left unchecked - a bare git tag, or a release
still flagged as pre-release, won't be picked up by `/releases/latest`. If
the check fails for any reason (offline, rate-limited, no stable release
published yet), it fails silently and the app works normally.

## Notes on compatibility

- The application uses CodeWalker Core for RPF parsing and serialization
- The output RPF is a new file, providing a simple rollback path
- Always keep a backup of your original `update.rpf` before using this tool
- This tool is designed specifically for Switch GTA V builds

## Credits

- [CodeWalker Core](https://github.com/dexyfex/CodeWalker) for RPF handling
  
## License

GNU General Public License v3.0 - See LICENSE file for details.
