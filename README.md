# Rebirth Paradox

> Unity portfolio project: a fantasy action game built around wave combat, character progression, and replayable challenge rewards.

**Rebirth Paradox** is a Unity game project focused on combat systems, scalable enemy waves, player upgrades, and a quest-style reward loop. This repository is presented as a portfolio code sample and is not a production release.

## Portfolio

| Item | Details |
| --- | --- |
| Engine | Unity |
| Language | C# |
| Target | Desktop and XR-oriented Unity project |
| Project type | Source-code portfolio / development snapshot |
| Status | Work in progress |

## What I Built

- Wave-based combat with melee and ranged enemies.
- Endless mode with progressive difficulty scaling.
- Boss encounters at regular wave intervals.
- Player health, XP, weapons, stats, upgrades, and inventory systems.
- Weapon selection and weapon merging workflows.
- Daily, weekly, and monthly quest periods with progress tracking.
- Reward claiming with gem/currency progression.
- Achievement tracking and unlock notifications.
- Save-data and scene bootstrap managers.
- UI flows for menus, upgrades, shops, rewards, character selection, settings, pause, and game over.
- XR and Meta Quest package integration points.

## Technical Highlights

The code is organized around focused managers and gameplay components rather than one large controller. Some representative areas are:

| Area | Representative code | Is Include |
| --- | --- | --- |
| Combat | `Assets/Scripts/Enemy/`, `Assets/Scripts/Weapon/`, `Assets/Scripts/Player/` | `Yes` |
| Wave progression | `Assets/Scripts/Manager/WaveManager.cs`, `Assets/Scripts/Manager/EndlessWaveManager.cs` | `Yes`
| Rewards | `Assets/Scripts/Manager/RewardManager.cs`, `Assets/Scripts/UI/QuestRewardPanel.cs` |  `No` |
| Persistence | `Assets/Scripts/Manager/ES3SaveManager.cs`, `Assets/Scripts/Manager/DataPersistenceManager.cs` | `No` |
| XR support | `Packages/manifest.json`, `Assets/XR/`, `Assets/Oculus/` | `No` |

## Screenshots and Video

- [Official web page](https://indiexrgames.com/rebirth-paradox/)
- [Store page](https://www.meta.com/experiences/rebirth-paradox/26113131518278262/)


| Combat | Quest rewards | Character / upgrade flow |
| --- | --- | --- |
| Screenshot coming soon | Screenshot coming soon | Screenshot coming soon |

## Download

- [Download the source ZIP](https://indie-xr-games.itch.io/rebirth-paradox) 
- [Browse the source code](https://github.com/livbogdan/Portfolio/tree/master)
- Playable PC and apk build: not included in this repository.

## What Was Removed or Excluded

This portfolio version intentionally excludes or does not publish:

- A final playable build or store package.
- Production credentials, platform secrets, and live service configuration.
- Private development files and machine-specific Unity state such as `Library/`, `Temp/`, `Logs/`, and `UserSettings/`.
- Final marketing media, because no approved screenshots or gameplay recording is currently stored in the repository.
- Commercial art, audio, fonts, plugins, and other paid assets from the downloadable portfolio package.
- Any unfinished prototype experiments that are not required to understand the core systems shown here.

## What Is Not Used in This Portfolio

The following are outside the scope of this code showcase and should not be treated as shipped features:

- Online multiplayer or networked gameplay.
- A public release pipeline or automated build distribution.
- A finalized monetization setup, even though the project contains purchase-related integration points.
- A guaranteed production-ready balance pass for endless waves, bosses, or rewards.
- Complete store compliance, localization, accessibility, and device certification.

## Running the Project

1. Install the Unity version configured for the project.
2. Open the repository as a Unity project.
3. Open the relevant scene from `Assets/Scenes/`.
4. Assign the required scene and prefab references in the Inspector.
5. Enter Play mode and review the Unity Console for missing references.

## Disclaimer and Asset Notice

This repository is **source code only** and is provided for portfolio and educational review. It is **not a complete game download and does not include paid or redistributable third-party assets**. Any asset names, package references, sample content, or placeholders visible in the project are subject to their original owners' licenses and must not be redistributed or used commercially without the required permissions.

Do not treat this repository as permission to use third-party models, textures, audio, fonts, plugins, SDKs, or trademarks.

