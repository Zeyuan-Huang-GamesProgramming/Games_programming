# Testing and Iteration Record

## Project Details

| Item | Detail |
| --- | --- |
| Project | **Deep Space Station: Last Evacuation** |
| Engine | Unity 2022.3.62f3c1 |
| Document version | 1.2 |
| Record date | Compiled on 26 May 2026 |
| Development period covered | Early May-26 May 2026 (student development record, repository history, and current verification) |
| Testing focus | Playable vertical slice stability, feedback, presentation, and professional project organisation |

## Purpose

This document records how the playable vertical slice has been checked and
improved during development. It separates completed technical verification from
final hands-on playtest checks that must be repeated on the submitted build. The
aim is to demonstrate testing, debugging, response to feedback, and honest
identification of outstanding work.

## Record Integrity

This document was assembled retrospectively on 26 May 2026. Work completed
before the first GitHub upload is recorded from the student's development
recollection and is labelled as an offline development record. Entries from
19 May onwards are also supported by repository history. These development
milestones do not claim that the full manual test matrix was executed on those
dates. Formal verification entries use the date on which each check was
actually recorded.

## Development and Iteration Timeline

| Date / period | Record basis | Development / testing significance |
| --- | --- | --- |
| Early May 2026 (approximately 1-7 May) | Student-reported offline development record; work began locally before the GitHub upload history. | Established the initial game direction: a first-person science-fiction evacuation experience set on a damaged station. Began constructing the Unity project and planning the player's movement, exploration, oxygen pressure and escape objective. |
| Second week of May 2026 (approximately 8-14 May) | Student-reported offline development record; retained here as a retrospective process entry. | Developed the early playable loop and station content: exploration areas, interactable objectives, hazards, doors and the escape route. Testing at this stage focused informally on whether the player could navigate the level and understand the core objective. |
| Mid-May 2026 (approximately 15-18 May) | Student-reported offline development record, immediately preceding the initial repository upload. | Prepared a shareable baseline version of the project and supporting instructions, consolidating the prototype before transferring development evidence to GitHub. |
| 19 May 2026 | GitHub commit [`ead095d`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/ead095d07b4e8e60f0961b18b60b58e95f8aa67b), `Add first version of Deep Space Station Unity project`. | Establishes the first repository baseline of the playable prototype, providing the reference point for later improvement and regression checking. |
| 19 May 2026 | GitHub commit [`7b062de`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/7b062de4d41d24554f10076c59591c05cbbce59f), `Translate Deep Space Station README to English`. | Improves project communication and presentation readiness. |
| 21 May 2026 | GitHub commit [`de09d69`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/de09d69cb4323dfc8931a5902af87060d01fa5ab), `Improve deep space station scene`. | Records a scene and presentation iteration beyond the initial baseline. |
| 21 May 2026 | GitHub commit [`225369d`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/225369d4f7d9cb3987b434fa775dd2455959b12a), `Use English project documentation`. | Records continued improvement of submission-facing documentation. |
| 25 May 2026 | GitHub commit [`04d0cca`](https://github.com/Zeyuan-Huang-GamesProgramming/Games_programming/commit/04d0ccaab53b87f8ceacde211ff0d69103d944e0), `Update Deep Space Station project`. | Shows a further project revision before final verification was documented. |
| 26 May 2026 | Current workspace verification and this testing record. | Records compilation, hierarchy verification, the remaining build-configuration issue, and the manual test sign-off still required. |

### Evidence Note

The early-May entries explain development work completed locally before the
project was first uploaded to GitHub. They are identified transparently as
retrospective student records rather than GitHub-confirmed checkpoints. If
dated screenshots, tutor feedback notes, previous builds, test sheets, or old
project files are available, they can be added to strengthen these entries
with supporting evidence.

## Status Key

| Status | Meaning |
| --- | --- |
| **PASS** | The stated check was carried out and the expected result was confirmed. |
| **IMPLEMENTED - SIGN-OFF PENDING** | The feature/fix is present and compiles, but a final manual playthrough result should be recorded before submission. |
| **OPEN ISSUE** | A problem has been identified and still requires action or final confirmation. |

## Recorded Verification Results

| ID | Date | Verification performed | Result | Evidence / outcome |
| --- | --- | --- | --- | --- |
| VT-01 | 26 May 2026 | Compiled the current runtime and editor code through `Assembly-CSharp-Editor.csproj`. | **PASS** | Build completed with `0 errors`. Eight `CS0162` warnings remain in the scene builder because constant configuration branches are intentionally inactive; these do not stop compilation. |
| VT-02 | 26 May 2026 | Checked the saved `Station_A` scene after the hierarchy organisation improvement. | **PASS** | The saved scene contains organised roots including `00_SYSTEMS`, `01_USER_INTERFACE`, `02_LIGHTING`, `03_TIMED_EVACUATION`, `04_ENDLESS_SURVIVAL`, and `99_UNSORTED`. |
| VT-03 | 26 May 2026 | Checked the Unity Editor log for the organisation operation. | **PASS** | Unity logged that the production-friendly scene hierarchy operation completed for `Assets/Scenes/Station_A.unity`. |
| VT-04 | 26 May 2026 | Inspected build-scene configuration in `ProjectSettings/EditorBuildSettings.asset`. | **OPEN ISSUE** | `m_Scenes` is currently empty. A final playable build cannot be treated as verified until `Station_A.unity` is added to Build Settings and an exported build is tested. |

## Functional Test Matrix

These tests cover the key game systems requested for the vertical slice. Tests
marked as pending should be executed again in a clean Unity Play Mode session
and then in the final exported build. A screenshot or short capture of the most
important results can be retained as supporting assessment evidence.

| Test ID | Feature under test | Test steps | Expected result | Current record |
| --- | --- | --- | --- | --- |
| FT-01 | Door opening animation and collision | Start Timed Evacuation; approach an available bulkhead; press `E`; watch the panel during movement; walk through after it finishes opening. Repeat with a locked door before its requirement is met. | An unlocked door visibly slides open instead of disappearing immediately; passage is blocked until movement completes, then becomes traversable. A locked door stays closed and gives an audible/visual denial message. | **IMPLEMENTED - SIGN-OFF PENDING.** `DoorController` now moves the panel using `Vector3.MoveTowards`, exposes an opening state, and only clears blockers when motion is complete. Code compiles successfully. |
| FT-02 | Mode selection and mode-specific rules | Launch scene; select `1` for Timed Evacuation; restart; select `2` for Endless Survival. Check spawning, HUD wording, objectives, timer/score, and escape-pod behaviour. | Timed mode begins at its intended spawn, shows a countdown and allows extraction after objectives. Endless mode begins in its arena, shows score/threat progression, and does not permit extraction. | **IMPLEMENTED - SIGN-OFF PENDING.** `GameManager` contains explicit `TimedEvacuation` and `EndlessSurvival` flows, mode selection, spawn routing, and mode-specific extraction rules. |
| FT-03 | Pause menu volume and mouse sensitivity persistence | Begin a mode; press `Esc`; lower and raise master volume; change sensitivity; resume and confirm mouse response; restart the scene and reopen settings. | Pause menu shows current values; volume changes audio immediately; sensitivity changes look control; saved values remain after scene reload. | **IMPLEMENTED - SIGN-OFF PENDING.** `HUDController` calls `GameSettings`, which stores both values through `PlayerPrefs`; `FirstPersonController` reads the saved sensitivity and `AudioListener.volume` receives saved volume. |
| FT-04 | Results screen and high-score/record storage | Complete a Timed run; check rating and fastest escape result; begin another run and compare records. In Endless mode, achieve a score, lose/end the run, and return to selection screen. | End screen reports the run clearly; a new best rating, fastest escape, high score, or longest run is recorded and remains visible on later visits. | **IMPLEMENTED - SIGN-OFF PENDING.** Record keys and persistence are implemented in `GameManager` through `PlayerPrefs`, with briefing/result/record UI hooks in `HUDController`. |
| FT-05 | Security robot patrol, detection, pursuit, and damage | Enter a robot patrol area out of sight; observe patrol. Move into detection range; sprint and crouch in separate attempts; break line of sight; allow a close-range attack once. | Robot patrols until detection, chases and warns on detection, damages the player at close range, searches briefly after losing sight, then returns to patrol. Sprinting increases risk and crouching reduces detection range. | **IMPLEMENTED - SIGN-OFF PENDING.** `SecurityRobot` contains patrol, line-of-sight detection, movement-state multipliers, search/give-up logic, attack damage, and endless threat scaling. |
| FT-06 | Oxygen drain, hazard multiplier, refill, and failure | Start Timed mode and monitor oxygen; enter a radiation leak; leave it; collect an oxygen canister; in a test run allow oxygen to reach zero. | Oxygen drains over time; drains faster inside the hazard; refilling increases available oxygen; zero oxygen triggers a clear failure screen and failure audio. | **IMPLEMENTED - SIGN-OFF PENDING.** `PlayerOxygen`, `HazardZone`, `OxygenCanister`, HUD feedback, and procedural warning/failure sounds are connected in code. |
| FT-07 | Organised scene hierarchy after tutor feedback | Open `Assets/Scenes/Station_A.unity`; inspect the root hierarchy; expand timed mode and locate a door, terminal, pickup, and hazard; expand endless mode and locate an enemy. | Major categories are visible at the root, and specific gameplay objects can be found rapidly without scrolling through a flat list. | **PASS.** Saved scene data and the Unity editor operation log confirm the reorganised hierarchy is present. |
| FT-08 | Final build launch and complete playthrough | Add `Assets/Scenes/Station_A.unity` to Build Settings; create a Windows build; launch outside the editor; complete one Timed run and start one Endless run. | Build launches without missing-scene errors; controls, UI, audio, settings, results and records behave as in the editor. | **OPEN ISSUE.** The Build Settings scene list is empty at the time of this record, so final build testing has not yet been completed. |

## Bug Fix and Improvement Log

| Iteration | Problem or feedback identified | Change made | Verification / assessment value |
| --- | --- | --- | --- |
| IT-01: Door feedback | The door interaction required more convincing physical feedback and a reliable opening transition. | Added animated panel motion, an explicit opening state, delayed blocker removal, locked-door denial feedback, and door audio trigger. | Implementation compiles; `FT-01` defines final playtest acceptance. Supports animation, collision and interaction criteria. |
| IT-02: UI presentation | Default text/UI presentation did not match the science-fiction setting strongly enough. | Converted HUD presentation to TextMesh Pro, applied a consistent display typeface and sci-fi panel treatment, and improved opening/result presentation. | Scene/UI migration was applied in Unity. Supports coherence and final-game polish. |
| IT-03: Game feedback | The game lacked sufficient audio response for player actions and threat states. | Added runtime procedural ambience and feedback sounds for doors, terminals, pickups, low oxygen, robots, damage, success and failure. | Audio system compiles and is connected to gameplay events; final listening check remains part of manual build sign-off. |
| IT-04: Usability | Player comfort/settings options were limited. | Added pause-menu master volume and mouse sensitivity controls with persistent saved values. | Settings persistence is implemented with `PlayerPrefs`; `FT-03` defines confirmation steps. Supports accessibility awareness and usability. |
| IT-05: Replay value and closure | Runs needed clearer purpose and replay motivation. | Added Timed and Endless modes, opening briefing, enhanced results output, and persisted best records/high scores. | Systems compile and are visible in implementation; `FT-02` and `FT-04` define final gameplay confirmation. |
| IT-06: Tutor hierarchy feedback | Tutor feedback identified that the Unity Hierarchy was too flat and that finding objects took too long. | Added a scene organiser and reorganised the saved scene into numbered functional roots and subgroups for architecture, interactables, pickups, hazards, enemies, dressing and labels. | **PASS:** `Station_A.unity` contains the organised structure and Unity logged the operation. Demonstrates a direct response to feedback and professional organisation. |

## Tutor Feedback Response: Hierarchy Organisation

The tutor noted that locating an individual object in the scene took too long
because numerous walls, floors, lights and gameplay objects appeared together
at the root of the Unity Hierarchy. This feedback was addressed by adding an
organisation tool to the editor workflow and applying it to the playable scene.

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

This improvement does not change level layout or gameplay rules. It improves
maintenance, debugging, demonstration readiness, and the ability to explain the
project professionally during assessment.

## Outstanding Work Before Submission

| Priority | Outstanding task | Why it matters | Planned verification |
| --- | --- | --- | --- |
| High | Add `Assets/Scenes/Station_A.unity` to Unity Build Settings and generate a Windows build. | A stable playable build is a central Final Game requirement. | Execute `FT-08` in the exported build and record pass/fail, build date and platform. |
| High | Run and record the pending manual functional tests in both modes. | Code presence is not the same as a completed gameplay test. | Replace pending statuses in `FT-01` to `FT-06` with observed results and screenshots where useful. |
| High | Commit the latest local implementation and this record to GitHub. | GitHub must show consistent progress, testing and iteration evidence. | Confirm the current files appear in the repository history with clear commit messages. |
| Medium | Update the README and run instructions to match current controls and features. | Current documentation describes an earlier version of the game. | Check that a new user can launch and understand the game using only the repository instructions. |

## Final Build Test Session Template

This section should be completed immediately after generating the submission
build.

| Item | Entry |
| --- | --- |
| Build date | _To be recorded_ |
| Platform | Windows 64-bit |
| Unity version | _To be recorded_ |
| Build folder/version | _To be recorded_ |
| Timed Evacuation full run result | _Pass / Fail and notes_ |
| Endless Survival start/score result | _Pass / Fail and notes_ |
| Audio/settings persistence result | _Pass / Fail and notes_ |
| Critical bugs found | _None or issue details_ |
| Fixes made after build test | _To be recorded_ |

## Evidence Locations

- Gameplay flow and persistent run records: `Assets/Scripts/Core/GameManager.cs`
- Procedural audio and persistent settings: `Assets/Scripts/Core/GameAudio.cs`
- HUD and settings controls: `Assets/Scripts/UI/HUDController.cs`
- Door interaction and animation: `Assets/Scripts/World/DoorController.cs`
- Robot behaviour: `Assets/Scripts/World/SecurityRobot.cs`
- Oxygen/failure handling: `Assets/Scripts/Player/PlayerOxygen.cs`
- Scene creation and hierarchy organiser: `Assets/Editor/DeepSpaceStationSceneBuilder.cs`
- Playable scene: `Assets/Scenes/Station_A.unity`
- Build configuration needing completion: `ProjectSettings/EditorBuildSettings.asset`
