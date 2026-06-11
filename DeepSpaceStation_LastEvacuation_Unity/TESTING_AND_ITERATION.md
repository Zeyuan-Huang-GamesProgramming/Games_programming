# Testing and Iteration Record

## Project Details

| Item | Detail |
| --- | --- |
| Project | **Deep Space Station: Last Evacuation** |
| Engine | Unity 2022.3.62f3c1 |
| Document version | 1.5 |
| Record date | Updated on 11 June 2026 |
| Development period covered | Early May-11 June 2026 |
| Testing focus | Playable vertical slice stability, build readiness, feedback, UI/audio polish, and professional project organisation |

## Purpose

This document records how the playable vertical slice was tested and improved during development. It shows the connection between design decisions, implementation changes, debugging, tutor feedback, and final build verification. The aim is to demonstrate that the project was not only built, but also checked, improved, and prepared as a stable final submission.

## Record Integrity

Early-May work was carried out locally before the first GitHub upload. Those entries are recorded transparently as retrospective student development notes. From 19 May onwards, the project also has GitHub history and local verification evidence. The final checks on 11 June 2026 separate three evidence types: Unity build configuration/build logs, standalone Windows launch logs, and student manual playtest sign-off for Timed Evacuation and Endless Survival.

The command-line build was run from a temporary copy of the same `Assets`, `Packages`, and `ProjectSettings` folders because the main Unity project was open in the editor. This avoided the Unity project lock while still testing the current project content.

## Development and Iteration Timeline

| Date / period | Record basis | Development / testing significance |
| --- | --- | --- |
| Early May 2026 (approximately 1-7 May) | Student-reported offline development record. | Established the initial game direction: a first-person science-fiction evacuation game set on a damaged station. Began planning movement, interaction, oxygen pressure, hazards, repair objectives, and the escape goal. |
| Second week of May 2026 (approximately 8-14 May) | Student-reported offline development record. | Built the first playable loop with exploration areas, doors, terminals, hazards, oxygen pressure, and an escape route. Informal testing focused on navigation clarity and whether the main objective was understandable. |
| Mid-May 2026 (approximately 15-18 May) | Student-reported offline development record. | Consolidated the prototype before uploading it to GitHub. Checked that the station layout, player controller, interactables, and basic mission flow were usable as a vertical slice baseline. |
| 19 May 2026 | GitHub commit [`ead095d`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/ead095d07b4e8e60f0961b18b60b58e95f8aa67b). | First repository baseline of the playable prototype. |
| 19 May 2026 | GitHub commit [`7b062de`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/7b062de4d41d24554f10076c59591c05cbbce59f). | README translated and improved for assessment communication. |
| 21 May 2026 | GitHub commits [`de09d69`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/de09d69cb4323dfc8931a5902af87060d01fa5ab) and [`225369d`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/225369d4f7d9cb3987b434fa775dd2455959b12a). | Improved the station scene and project documentation. |
| 25-26 May 2026 | Workspace verification and testing record. | Recorded compilation checks, hierarchy organisation, and a formal test matrix for the core systems. |
| 2-4 June 2026 | Local implementation and GitHub update cycle. | Added clearer failure reasons, low-resource warning feedback, risk-reward supply caches, and stronger gameplay pressure. |
| 8-9 June 2026 | Local implementation and GitHub update cycle. | Reworked the main menu, options/how-to-play/credits panels, final result screen layout, background music, footstep audio, and pause-menu exit flow. |
| 11 June 2026 | Final build verification and this updated testing record. | Generated a Windows 64-bit build, launched the standalone build successfully, recorded final manual sign-off results for Timed Evacuation and Endless Survival, and updated this document to remove pending test statuses. |

## Status Key

| Status | Meaning |
| --- | --- |
| **PASS** | The stated check was carried out and the expected result was confirmed. |
| **PASS - STUDENT PLAYTEST SIGN-OFF** | The feature was confirmed through a recorded student hands-on playtest in Play Mode or the Windows build. |
| **PASS - BUILD/LAUNCH VERIFIED** | The feature was included in the exported Windows build or confirmed through standalone launch/log evidence. |
| **PARTIAL PASS** | The issue described by the test was fixed, but a wider manual playthrough or supporting screenshot/video would strengthen the evidence. |

## Recorded Verification Results

| ID | Date | Verification performed | Result | Evidence / outcome |
| --- | --- | --- | --- | --- |
| VT-01 | 26 May 2026 | Compiled the runtime and editor code through the Unity-generated C# projects. | **PASS** | Code compiled with no blocking errors. Earlier unreachable-code warnings in the scene builder were non-blocking editor-script warnings. |
| VT-02 | 26 May 2026 | Checked the saved `Station_A` scene after the hierarchy organisation improvement. | **PASS** | The saved scene contains organised roots including `00_SYSTEMS`, `01_USER_INTERFACE`, `02_LIGHTING`, `03_TIMED_EVACUATION`, `04_ENDLESS_SURVIVAL`, and `99_UNSORTED`. |
| VT-03 | 26 May 2026 | Checked the Unity Editor log for the organisation operation. | **PASS** | Unity logged that the production-friendly hierarchy operation completed for `Assets/Scenes/Station_A.unity`. |
| VT-04 | 11 June 2026 | Inspected build-scene configuration in `ProjectSettings/EditorBuildSettings.asset`. | **PASS** | `Assets/Scenes/Station_A.unity` is listed and enabled in Build Settings. |
| VT-05 | 11 June 2026 | Exported a Windows 64-bit build using Unity 2022.3.62f3c1 batch mode from a temporary project copy. | **PASS - BUILD/LAUNCH VERIFIED** | Build output was generated at `Builds/Windows/DeepSpaceStation_LastEvacuation.exe`. Build log ends with `Exiting batchmode successfully now!`. |
| VT-06 | 11 June 2026 | Launched the exported Windows build for a standalone smoke test. | **PASS - BUILD/LAUNCH VERIFIED** | The process stayed running after 15 seconds and wrote a Player log. The log confirms engine startup and BGM playback: `Deep Space Station BGM playing: TenseFutureLoop`. |
| VT-07 | 11 June 2026 | Final student manual gameplay sign-off: Timed Evacuation full run and Endless Survival start test. | **PASS - STUDENT PLAYTEST SIGN-OFF** | Timed Evacuation was completed through the main objective flow and result screen. Endless Survival was started successfully and confirmed to show endless-mode HUD/score/threat behaviour. This is a manual gameplay record rather than an automated test. |

## Functional Test Matrix

These tests cover the key game systems required for the vertical slice. The final records below replace the previous implementation-only pending statuses with completed pass results.

| Test ID | Feature under test | Test steps | Expected result | Final record |
| --- | --- | --- | --- | --- |
| FT-01 | Door opening animation and collision | Start Timed Evacuation; approach an available bulkhead; press `E`; watch the panel during movement; walk through after it finishes opening. Repeat with a locked door before its requirement is met. | An unlocked door visibly slides open; passage is blocked until movement completes, then becomes traversable. A locked door stays closed and gives audible/visual denial feedback. | **PASS.** Door animation, blocker timing, locked-door denial feedback, and door audio behaved correctly in final testing. |
| FT-02 | Mode selection and mode-specific rules | Launch the game; select Timed Evacuation; restart; select Endless Survival. Check spawn, HUD wording, objectives, timer/score, and escape-pod behaviour. | Timed mode starts at the intended spawn with countdown/objectives/extraction. Endless mode starts in its arena with score/threat progression and no normal extraction objective. | **PASS.** Both modes launched correctly. Timed Evacuation and Endless Survival used separate spawns, HUD states, and rule sets. |
| FT-03 | Pause menu volume and mouse sensitivity persistence | Begin a mode; press `Esc`; change master volume and look sensitivity; resume; restart scene and reopen settings. | Settings update immediately and remain saved after reload. | **PASS.** Volume and look sensitivity changed through the UI, affected gameplay/audio, and persisted through `PlayerPrefs`. |
| FT-04 | Results screen and high-score/record storage | Complete a Timed run and check rating/fastest escape. In Endless mode, achieve a score and end/lose the run. Return to menu and inspect records. | End screen clearly reports the run and records best rating, fastest escape, high score, or longest run. | **PASS.** Results screen displayed clear run data. Timed and Endless records persisted and were visible from the menu/archive display. |
| FT-05 | Security robot patrol, detection, pursuit, and damage | Enter a robot patrol area out of sight; move into detection range; test sprint/crouch risk; break line of sight; allow one close-range attack. | Robot patrols, detects, chases, damages, searches after losing sight, and returns to patrol. Sprinting increases risk and crouching reduces detection range. | **PASS.** Robot AI patrol, line-of-sight detection, pursuit, attack damage, search behaviour, and movement-state detection modifiers worked as intended. |
| FT-06 | Oxygen drain, hazard multiplier, refill, and failure | Start Timed mode; monitor oxygen; enter/leave a hazard; collect an oxygen canister; allow oxygen to reach zero in a test run. | Oxygen drains, hazard increases drain, refill restores oxygen, and zero oxygen triggers a clear failure result. | **PASS.** Oxygen pressure, hazard drain, refill pickup, low-oxygen warnings, and oxygen-failure result screen all worked. |
| FT-07 | Organised scene hierarchy after tutor feedback | Open `Assets/Scenes/Station_A.unity`; inspect root hierarchy; locate a door, terminal, pickup, hazard, and enemy quickly. | Major categories are visible at the root and objects are easy to locate without scrolling through a flat list. | **PASS.** The scene is organised into numbered functional groups for systems, UI, lighting, timed content, endless content, and unsorted fallback objects. |
| FT-08 | Final Windows build launch and playthrough | Create Windows build; launch outside editor; complete one Timed run and start one Endless run. | Build launches without missing-scene errors; controls, UI, audio, settings, results, and records behave as in the editor. | **PASS.** `Station_A.unity` is included in Build Settings, the Windows build exported successfully, standalone launch succeeded, and student manual sign-off recorded one Timed Evacuation full run plus one Endless Survival start/score test. |
| FT-09 | Main menu, options, how-to-play, and credits layout | Open main menu; click New Game, Options, How To Play, and Credits; use Back buttons at different resolutions. | Menu buttons match their visual hitboxes and subpanels remain readable on the target game view. | **PASS.** Main menu background, button hitboxes, options controls, how-to-play back button, and credits back button were corrected and verified. |
| FT-10 | Full-game audio pass | Start game, move, interact with doors/terminals/pickups, trigger robot detection, pause/change volume, and launch final build. | Background music plays throughout; footsteps and interaction sounds support feedback; volume control affects audio. | **PASS.** BGM, footsteps, interaction feedback, warning sounds, result sounds, and volume control were confirmed. Standalone Player log also recorded BGM playback. |
| FT-11 | Pause menu exit to main menu | Start a mode, press `Esc`, use `EXIT TO MAIN MENU`, then start another mode. | Game returns cleanly to the main menu/mode flow without freezing or requiring application quit. | **PASS.** Pause menu exit returned to the main menu flow and allowed a new run to be started. |
| FT-12 | Result/failure screen readability | Trigger health, oxygen, and mission failure cases; inspect title, reason, score, and restart/menu prompts. | Failure reason is clear and the result UI fits the target game view. | **PASS.** Failure reasons, next-attempt advice, score data, and result layout were readable after final UI layout fixes. |

## Bug Fix and Improvement Log

| Iteration | Problem or feedback identified | Change made | Verification / assessment value |
| --- | --- | --- | --- |
| IT-01: Door feedback | Door interaction needed stronger physical feedback and reliable collision timing. | Added animated panel motion, explicit opening state, delayed blocker removal, locked-door denial feedback, and door audio trigger. | **PASS:** Verified through `FT-01`. Supports animation, collision, interaction, and feedback criteria. |
| IT-02: UI presentation | Default UI did not strongly match the science-fiction setting. | Converted HUD/menu presentation to TextMesh Pro, applied consistent sci-fi typography, and improved briefing/result presentation. | **PASS:** Verified through menu, HUD, and result-screen checks. Improves polish and player clarity. |
| IT-03: Audio feedback | The game originally had limited sound outside task completion. | Added continuous BGM, procedural ambience, footsteps, door/terminal/pickup feedback, warning sounds, robot alert, damage, success, and failure audio. | **PASS:** Verified through `FT-10` and standalone Player log. Improves immersion and game feel. |
| IT-04: Usability and accessibility | Player comfort/settings options were limited. | Added master volume and look sensitivity controls in both main menu options and pause menu, with automatic saving. | **PASS:** Verified through `FT-03`. Supports usability and accessibility awareness. |
| IT-05: Replay value and closure | The game needed clearer structure and replay motivation. | Added Timed Evacuation and Endless Survival modes, opening briefing, enhanced result output, and persistent best records/high scores. | **PASS:** Verified through `FT-02` and `FT-04`. Strengthens vertical-slice completeness. |
| IT-06: Tutor hierarchy feedback | Tutor feedback identified that the Unity Hierarchy was too flat and slow to navigate. | Added a scene organiser and reorganised the saved scene into numbered functional roots and subgroups for architecture, interactables, pickups, hazards, enemies, dressing, and labels. | **PASS:** Verified through `FT-07`. Demonstrates response to feedback and professional organisation. |
| IT-07: Risk-reward gameplay | Resource pickups were useful but did not always create interesting player choices. | Added optional supply caches containing oxygen, medkit, and battery resources in riskier side locations. | **PASS:** Verified in Timed route testing. Adds decision-making without increasing scope too much. |
| IT-08: Failure clarity and warnings | Failure needed clearer cause-and-effect before and after the player lost. | Added explicit failure reasons, low oxygen warnings, time-critical feedback, suit-critical feedback, and clearer result advice. | **PASS:** Verified through `FT-06` and `FT-12`. Improves readability, fairness, and assessment presentation. |
| IT-09: Main menu polish | The old menu layout looked unfinished and some hitboxes did not match visuals. | Added a dark sci-fi main menu background, aligned button hitboxes, fixed Options/How To Play/Credits panels, and removed the unnecessary Quit button. | **PASS:** Verified through `FT-09`. Improves first impression and presentation quality. |
| IT-10: Pause flow and final build readiness | The player needed a safe way to leave a run without closing the application. | Added `EXIT TO MAIN MENU` to pause menu and verified final Windows build export/launch. | **PASS:** Verified through `FT-08` and `FT-11`. Improves stability and demo readiness. |

## Tutor Feedback Response: Hierarchy Organisation

The tutor noted that locating an individual object in the scene took too long because many walls, floors, lights, and gameplay objects appeared together at the root of the Unity Hierarchy. This was addressed by adding an editor organisation workflow and applying it to the playable scene.

The new root structure is:

```text
00_SYSTEMS
01_USER_INTERFACE
02_LIGHTING
03_TIMED_EVACUATION
04_ENDLESS_SURVIVAL
99_UNSORTED
```

Inside each game mode, objects are further separated into:

```text
00_ARCHITECTURE
01_INTERACTABLES
02_PICKUPS
03_HAZARDS
04_ENEMIES
05_DRESSING
06_WORLD_LABELS
```

This improvement does not change gameplay rules. It improves maintenance, debugging, demonstration readiness, and the ability to explain the project professionally during assessment.

## Completed Final Submission Checks

| Priority | Final task | Result | Evidence |
| --- | --- | --- | --- |
| High | Add `Assets/Scenes/Station_A.unity` to Unity Build Settings. | **PASS** | `ProjectSettings/EditorBuildSettings.asset` lists the scene as enabled. |
| High | Generate a Windows 64-bit playable build. | **PASS** | `Builds/Windows/DeepSpaceStation_LastEvacuation.exe` was generated on 11 June 2026. |
| High | Launch the exported build outside the Unity editor. | **PASS** | Standalone launch smoke test stayed running after 15 seconds and wrote a Player log. |
| High | Run and record final functional sign-off in both modes. | **PASS** | `FT-01` to `FT-12` are recorded as pass, including Timed Evacuation full run and Endless Survival start test. |
| Medium | Keep documentation aligned with current controls/features. | **PASS** | README, testing record, asset documentation, final report/supporting documents, and GitHub/Kanban evidence were updated during the final polish cycle. |

## Manual Test Evidence Note

The final Windows build export and launch are supported by local log files. The detailed gameplay checks for doors, mode selection, pause/settings, results, robot behaviour, oxygen/hazards, and final playthrough are recorded as student manual playtest sign-off because these behaviours require keyboard/mouse gameplay observation rather than a simple automated command-line check.

For presentation/submission evidence, the strongest supporting material is:

- a short recording showing Timed Evacuation from start to result screen;
- a short recording showing Endless Survival launch, score/threat HUD, and robot pressure;
- screenshots of the final main menu, pause settings, and result screen;
- the build log and launch log listed in the evidence section below.

## Final Build Test Session

| Item | Entry |
| --- | --- |
| Build date | 11 June 2026 |
| Platform | Windows 64-bit |
| Unity version | Unity 2022.3.62f3c1 |
| Build folder/version | `Builds/Windows/DeepSpaceStation_LastEvacuation.exe` |
| Build method | Unity batchmode build from temporary project copy using current `Assets`, `Packages`, and `ProjectSettings` |
| Build result | **PASS** |
| Build evidence | `Logs/FinalBuild_2026-06-11.log` ends with `Exiting batchmode successfully now!` |
| Standalone launch result | **PASS** - process launched and stayed running after 15 seconds |
| Standalone launch evidence | `Logs/FinalBuild_PlayerLaunch_2026-06-11.log` confirms Unity startup and BGM playback |
| Timed Evacuation full run result | **PASS** - final route, mission objectives, escape/result flow, records, UI, and audio were confirmed |
| Endless Survival start/score result | **PASS** - endless spawn, score/threat HUD, robot pressure, and mode-specific loop were confirmed |
| Audio/settings persistence result | **PASS** - BGM, footsteps, interaction audio, volume setting, and look sensitivity persistence were confirmed |
| Critical bugs found | None blocking final submission |
| Fixes made after build test | No blocking fixes required after final sign-off; previous UI/audio/pause/menu fixes are recorded above |

## Evidence Locations

- Gameplay flow and persistent run records: `Assets/Scripts/Core/GameManager.cs`
- Procedural audio, BGM, footsteps, and persistent settings: `Assets/Scripts/Core/GameAudio.cs`
- HUD, menus, result screen, and settings controls: `Assets/Scripts/UI/HUDController.cs`
- Door interaction and animation: `Assets/Scripts/World/DoorController.cs`
- Robot behaviour: `Assets/Scripts/World/SecurityRobot.cs`
- Oxygen/failure handling: `Assets/Scripts/Player/PlayerOxygen.cs`
- Scene creation and hierarchy organiser: `Assets/Editor/DeepSpaceStationSceneBuilder.cs`
- Playable scene: `Assets/Scenes/Station_A.unity`
- Build configuration: `ProjectSettings/EditorBuildSettings.asset`
- Final Windows build: `Builds/Windows/DeepSpaceStation_LastEvacuation.exe`
- Build log: `Logs/FinalBuild_2026-06-11.log`
- Standalone launch log: `Logs/FinalBuild_PlayerLaunch_2026-06-11.log`
