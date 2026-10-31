<h1 align="center">Overcooked-Style Movement</h1>

<p align="center">
  Responsive turning and Overcooked-inspired dashing for the locally owned player in <a href="https://store.steampowered.com/app/3024130/">PAO PAO</a>.
</p>

<p align="center">
  <a href="https://store.steampowered.com/app/3024130/"><img alt="PAO PAO" src="https://img.shields.io/badge/PAO%20PAO-Steam%203024130-1b2838?logo=steam&logoColor=white"></a>
  <img alt="Version 1.0.0" src="https://img.shields.io/badge/version-1.0.0-2563eb">
  <img alt="BepInEx 6 IL2CPP" src="https://img.shields.io/badge/BepInEx-6%20IL2CPP-5c2d91">
  <a href="LICENSE"><img alt="MIT License" src="https://img.shields.io/badge/license-MIT-16a34a"></a>
</p>

## Features

| Feature | Default behavior |
|---|---|
| Normal turning | `2.5×` original local-player turn sharpness |
| Dash peak speed | `3.0×` sprint speed |
| Dash duration | `0.3` seconds |
| Dash profile | Sinusoidal decay into live movement input |
| Dash steering | Direction changes and full reversals allowed |
| Dash cooldown | `0.4` seconds |
| Dash turn speed | `20` radians per second |

Only the locally owned player's movement is adjusted.

## Install

1. Install [BepInEx 6 Unity IL2CPP Windows x64](https://builds.bepinex.dev/projects/bepinex_be) in the PAO PAO game directory.
2. Start and close PAO PAO once so BepInEx can generate its IL2CPP interop assemblies.
3. Download `OvercookedStyleMovement.dll` from [Releases](https://github.com/Satoheii/PaoPao-Overcooked-Style-Movement/releases) and copy it to `BepInEx\plugins`.
4. Start PAO PAO. The configuration file will be created at `BepInEx\config\com.satohei.paopao.overcookedstylemovement.cfg`.

## Configuration

| Setting | Default | Range | Description |
|---|---:|---:|---|
| `Precise Turning.Enabled` | `true` | `true` / `false` | Enables faster local-player turning. |
| `TurnSharpnessMultiplier` | `2.5` | `0.1`–`20.0` | Multiplier applied to original turn sharpness. |
| `Overcooked Dash.Enabled` | `true` | `true` / `false` | Enables the timed dash velocity blend. |
| `PeakSpeedMultiplier` | `3.0` | `1.0`–`5.0` | Peak dash speed relative to sprint speed. |
| `DashCooldown` | `0.4` | `0.0`–`5.0` | Minimum time between dash starts. |
| `TurnSpeed` | `20.0` | `1.0`–`100.0` | Dash turning speed in radians per second. |

The `0.3`-second duration and sinusoidal profile are fixed. Edit the configuration while the game is closed, then restart the game.

## Compatibility and Help

Tested with PAO PAO Steam build `26220223`, Unity `6000.3.12f1`, IL2CPP metadata `39`, and plugin version `1.0.0`.

PAO PAO is an Early Access game, so updates may require a new plugin build. If movement is unchanged or behaves unexpectedly:

- Confirm that the DLL is directly inside `BepInEx\plugins` and both feature-specific `Enabled` settings are `true`.
- Test with the locally owned character and refresh BepInEx interop assemblies after game updates.
- Check `BepInEx\LogOutput.log` for `Overcooked-Style Movement 1.0.0 loaded.` or an error.

Report problems through [GitHub Issues](https://github.com/Satoheii/PaoPao-Overcooked-Style-Movement/issues) with the game build, BepInEx and plugin versions, configuration, reproduction steps, and relevant log lines. Remove personal information from logs before posting.

## Uninstall

Close the game, delete `BepInEx\plugins\OvercookedStyleMovement.dll`, and optionally delete its configuration file.

## Building from source

Install the .NET 6 SDK or newer, install BepInEx 6 Unity IL2CPP in a local copy of PAO PAO, and start the game once. The project uses local references from `BepInEx\core` and `BepInEx\interop`; they are not NuGet packages.

Pass the game directory containing `PaoPao.exe` when building:

```bat
dotnet build OvercookedStyleMovement.csproj --configuration Release -p:PaoPaoGameDir="C:\Program Files (x86)\Steam\steamapps\common\PaoPao"
```

Alternatively, set `PAOPAO_GAME_DIR`:

```powershell
$env:PAOPAO_GAME_DIR = 'C:\Program Files (x86)\Steam\steamapps\common\PaoPao'
dotnet build OvercookedStyleMovement.csproj --configuration Release
```

`PaoPaoGameDir` takes priority over `PAOPAO_GAME_DIR`. The plugin is written to `bin\Release\OvercookedStyleMovement.dll`. A `Missing build reference` error means the game directory, BepInEx installation, or generated interop assemblies are incomplete.

The repository does not include, download, or copy BepInEx or generated game assemblies. Do not redistribute those dependencies.

## License

[MIT](LICENSE). This is an unofficial fan-made mod and is not affiliated with or endorsed by ArtDock or Dreland Enterprises.
