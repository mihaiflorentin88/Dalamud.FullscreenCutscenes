#!/usr/bin/env python3
import argparse
import json
import os
import time
import xml.etree.ElementTree as ET
from pathlib import Path


PROJECT_PATH = Path("Dalamud.FullscreenCutscenes/Dalamud.FullscreenCutscenes.csproj")
MANIFEST_PATH = Path("Dalamud.FullscreenCutscenes/Dalamud.FullscreenCutscenes.json")
PLUGIN_ZIP_NAME = "latest.zip"
DALAMUD_API_LEVEL = 15


def read_project_version() -> str:
    root = ET.parse(PROJECT_PATH).getroot()
    version = root.findtext("./PropertyGroup/Version")
    if not version:
        raise RuntimeError(f"Could not find <Version> in {PROJECT_PATH}")

    return version


def main() -> None:
    parser = argparse.ArgumentParser(description="Write a Dalamud custom plugin repository JSON.")
    parser.add_argument("--tag", required=True, help="Release tag, for example v1.0.0.4.")
    parser.add_argument("--output", required=True, type=Path, help="Output repo JSON path.")
    parser.add_argument(
        "--repository",
        default=os.environ.get("GITHUB_REPOSITORY", "mihaiflorentin88/Dalamud.FullscreenCutscenes"),
        help="GitHub repository in owner/name form.",
    )
    args = parser.parse_args()

    version = read_project_version()
    tag_version = args.tag[1:] if args.tag.startswith("v") else args.tag
    if tag_version != version:
        raise RuntimeError(f"Tag {args.tag} does not match project version {version}")

    manifest = json.loads(MANIFEST_PATH.read_text(encoding="utf-8-sig"))
    repo_url = f"https://github.com/{args.repository}"
    download_url = f"{repo_url}/releases/download/{args.tag}/{PLUGIN_ZIP_NAME}"

    entry = {
        "Author": manifest["Author"],
        "Name": manifest["Name"],
        "Punchline": manifest["Punchline"],
        "Description": manifest["Description"],
        "InternalName": manifest["InternalName"],
        "AssemblyVersion": version,
        "RepoUrl": repo_url,
        "ApplicableVersion": manifest.get("ApplicableVersion", "any"),
        "DalamudApiLevel": DALAMUD_API_LEVEL,
        "TestingDalamudApiLevel": DALAMUD_API_LEVEL,
        "IsHide": False,
        "IsTestingExclusive": False,
        "DownloadCount": 0,
        "LastUpdate": int(time.time()),
        "DownloadLinkInstall": download_url,
        "DownloadLinkTesting": download_url,
        "DownloadLinkUpdate": download_url,
        "IconUrl": f"https://raw.githubusercontent.com/{args.repository}/main/Dalamud.FullscreenCutscenes/icon.png",
        "Tags": manifest.get("Tags", []),
    }

    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps([entry], indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
