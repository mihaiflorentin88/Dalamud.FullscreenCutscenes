# Ultrawide Cutscenes

Dalamud plugin that removes the letterboxing bars shown during cutscenes on ultrawide monitors.

This repository is a maintained fork. Use this fork for current builds:

```sh
git clone https://github.com/mihaiflorentin88/Dalamud.FullscreenCutscenes.git
cd Dalamud.FullscreenCutscenes
```

## Normal Install

After a tagged release has been published, install the plugin through Dalamud by adding this custom repository URL:

```text
https://raw.githubusercontent.com/mihaiflorentin88/Dalamud.FullscreenCutscenes/gh-pages/repo.json
```

1. Launch the game.
2. Open Dalamud settings with `/xlsettings`.
3. Go to `Experimental`.
4. Add the URL above to `Custom Plugin Repositories`.
5. Save and close settings.
6. Open the plugin installer with `/xlplugins`.
7. Search for `Ultrawide Cutscenes Fork`.
8. Install and enable it.

## Development Install

Use this flow when testing local changes before pushing a release.

Requirements:

- Docker
- XIVLauncher with Dalamud enabled

Build the Docker build environment:

```sh
docker build -t dalamud-fullscreen-cutscenes-build .
```

Build the plugin and write the output into `./artifacts`:

```sh
mkdir -p artifacts
docker run --rm \
  --user "$(id -u):$(id -g)" \
  -e HOME=/tmp \
  -v "$PWD:/src" \
  -v "$HOME/.xlcore/dalamud/Hooks/dev:/dalamud:ro" \
  -v "$PWD/artifacts:/out" \
  dalamud-fullscreen-cutscenes-build
```

If your Dalamud dev files are somewhere else, change the second volume mount to point at the directory that contains `Dalamud.dll`.

Install the local build:

1. Launch the game.
2. Open Dalamud settings with `/xlsettings`.
3. Go to `Experimental`.
4. Add the full path to `artifacts/Dalamud.FullscreenCutscenes.MihaiFork.dll` to `Dev Plugin Locations`.
5. Open the plugin installer with `/xlplugins`.
6. Go to `Dev Tools` > `Installed Dev Plugins`.
7. Enable `Ultrawide Cutscenes Fork`.

## Releasing

GitHub Actions builds the plugin on pull requests and pushes to `main`. It publishes installable Dalamud releases only for version tags.

Before publishing:

1. Update `<Version>` in `Dalamud.FullscreenCutscenes/Dalamud.FullscreenCutscenes.csproj`.
2. Commit the changes locally when ready.
3. Push the branch to GitHub.
4. Create and push a matching tag:

```sh
git tag v1.0.0.4
git push origin v1.0.0.4
```

The tag version must match the csproj version. For example, tag `v1.0.0.4` requires `<Version>1.0.0.4</Version>`.

The release workflow will:

- build the plugin against Dalamud API 15;
- upload `latest.zip` to the GitHub Release;
- publish `repo.json` to the `gh-pages` branch for Dalamud installs.

This fork intentionally uses the unique internal plugin name `Dalamud.FullscreenCutscenes.MihaiFork`. Dalamud filters third-party repositories that try to override an official plugin's `InternalName`, so this must not be changed back to `Dalamud.FullscreenCutscenes`.

The repository must allow GitHub Actions to write releases and branches:

`Settings` > `Actions` > `General` > `Workflow permissions` > `Read and write permissions`.

## Usage

Toggle the plugin:

```text
/pcutscenes
```

Explicitly enable or disable it:

```text
/pcutscenes true
/pcutscenes false
```

## Notes

This plugin intentionally changes how cutscenes are framed. You may see things outside the intended camera view, including hidden actors, objects loading in, or animation states that are normally covered by letterboxing.
