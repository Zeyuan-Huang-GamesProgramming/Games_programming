# Deep Space Station: Last Evacuation

A Unity first-person science-fiction survival evacuation game. The player explores a damaged space station, restores critical systems, manages oxygen and time pressure, avoids hazards and security robots, and escapes through the evacuation pod.

## Game Concept

The project is a focused vertical slice: one polished section of a larger imagined game, with a clear objective, readable rules, and a complete win/lose loop.

The main design idea is **pressure-based exploration**. The player is always balancing route planning, oxygen, repair priorities, locked access, optional supply risk, and robot threat. This creates tension without requiring a very large level or unrealistic coursework scope.

## Intended Player Experience

The player should feel isolated inside a damaged station, but not confused. The game uses HUD prompts, objective text, warning colours, audio feedback, scanner information, and clear result screens to help the player understand danger and progress.

The intended emotional flow is:

1. Enter the station and understand the emergency.
2. Explore and repair systems under time and oxygen pressure.
3. Decide whether optional supplies are worth the risk.
4. Avoid or escape robot patrols.
5. Reach the escape pod or fail with a clear reason.

## Core Mechanics

- Explore the station in first person.
- Interact with doors, terminals, pickups, oxygen canisters, and the escape pod.
- Repair terminals through calibration and stabilisation.
- Manage oxygen, suit health, scanner battery, medkits, batteries, and supplies.
- Use the scanner to identify nearby interactables and security robots.
- Avoid hazards and robot line-of-sight detection.
- Escape in Timed Evacuation or survive for score in Endless Survival.

## How to Run

1. Open this project folder with Unity Hub.
2. Recommended Unity version: `2022.3 LTS`; the project was last verified with `2022.3.62f3c1`.
3. Wait for Unity to import the project on first launch.
4. Open the playable scene: `Assets/Scenes/Station_A.unity`.
5. Press Play.

If the scene needs to be rebuilt, use:

```text
Deep Space Station -> Build Playable Scene
```

For detailed setup instructions, see `UNITY_RUN.md`.

## Game Modes

### Timed Evacuation

The player starts in the damaged station, repairs required systems, manages oxygen and time, avoids security robots, and reaches the escape pod before the countdown ends.

### Endless Survival

The player starts in a separate survival arena and keeps repairing terminals for score while the threat level increases. The objective is no longer extraction, but survival and high score.

## Controls

- `WASD`: Move
- `Mouse`: Look around
- `Left Shift`: Sprint
- `C`: Crouch
- `Space`: Jump
- `E`: Interact
- `Q`: Hold scanner mode
- `Tab` / `I`: Toggle backpack/objective display
- `H`: Use medkit
- `B`: Use battery
- `Esc`: Pause or resume
- `R`: Restart after winning or losing

## Implemented Features

- First-person movement and camera control.
- Raycast interaction system.
- Timed Evacuation and Endless Survival modes.
- Oxygen, health, scanner battery, medkit, battery, and supply systems.
- Countdown, oxygen, health, time, and hazard-based failure conditions.
- Repair terminals with calibration and stabilisation interactions.
- Key-item and repair-gated station doors.
- Animated door opening and locked-door feedback.
- Radiation and vent hazard areas.
- Optional risk-reward supply caches.
- Security robot patrol, detection, pursuit, attack, and search behaviour.
- Scanner mode for nearby interactables and robot awareness.
- HUD, objectives, prompts, backpack, pause menu, options, and end screens.
- Explicit failure reasons, warning states, and next-attempt advice.
- Background music, footsteps, interaction sounds, warnings, robot alerts, success, and failure audio.
- High-score and best-record persistence.
- Unity Editor tool for generating the playable station scene.
- Organised Unity hierarchy for easier inspection and assessment.

## Design Rationale

The game uses a small number of connected systems rather than many unfinished features. Oxygen pressure makes movement choices meaningful. Repair terminals give the player short interaction goals. Locked doors and key items create route structure. Hazards and robots add risk. Optional supply caches add decision-making because the safest path is not always the most rewarding path.

The final game is intended to feel complete for its size: the controls work, the rules are visible, feedback is immediate, and the player can understand why they won or lost.

## Accessibility and Usability

Accessibility and usability considerations include:

- clear objective text and interaction prompts;
- readable TextMesh Pro HUD and sci-fi display font;
- master volume control;
- mouse sensitivity control;
- pause menu access;
- warning text for low oxygen, low time, and low suit health;
- explicit failure reasons in the result screen;
- scanner feedback for nearby objects and robots;
- audio cues for interaction, danger, success, and failure.

## Legal, Ethical, Social, and Security Considerations

External assets are credited and separated from original project work in `THIRD_PARTY_ASSETS.md`. The project uses downloaded free assets responsibly for environment presentation while the gameplay systems, interaction logic, UI integration, testing records, and design iteration are the main original coursework contribution.

This project does not use online accounts, networking, telemetry, or user data collection. Persistent settings and records are stored locally with Unity `PlayerPrefs`.

## Supporting Documentation

- `UNITY_RUN.md`: detailed run and build instructions.
- `TESTING_AND_ITERATION.md`: testing, debugging, final build verification, and improvement record.
- `THIRD_PARTY_ASSETS.md`: third-party asset attribution and distribution notes.
