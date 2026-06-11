# Deep Space Station: Last Evacuation

A Unity coursework project for Games Programming.

## Project Overview

**Deep Space Station: Last Evacuation** is a first-person science-fiction survival evacuation game. The player explores a damaged space station, repairs critical systems, manages oxygen and suit health, avoids security robots and environmental hazards, and reaches the escape pod before the situation becomes impossible.

The project is designed as a focused vertical slice rather than a large unfinished game. It presents one coherent playable scenario with a clear beginning, pressure-based middle, and win/lose end state.

## One-Sentence Game Idea

A stranded crew member must restore enough station systems to escape while balancing oxygen, time pressure, locked access routes, limited supplies, and robot patrol risk.

## Intended Player Experience

The intended experience is tense but readable. The player should feel that the station is dangerous and deteriorating, but the game should still provide clear feedback about what is happening and what to do next. The design aims for pressure, route planning, and short-term decision-making instead of large-scale exploration.

The main design idea is **pressure-based exploration**: the player is always balancing route planning, oxygen, repair priorities, locked access, optional supply risks, and robot threat.

## Core Design Principles

- **Clear objective:** repair key systems, survive, and reach the escape pod.
- **Resource pressure:** oxygen, health, scanner battery, time, and supplies create meaningful tension.
- **Readable feedback:** HUD warnings, prompts, sounds, failure reasons, and result screens explain consequences clearly.
- **Focused scope:** the game is built as a polished vertical slice, not an oversized open-world concept.
- **Risk and reward:** optional supply caches can help the player, but they require leaving the safest route.
- **Replay value:** Timed Evacuation provides a completion challenge, while Endless Survival supports score-based replay.

## Implemented Vertical Slice

- First-person movement, sprinting, crouching, jumping, and mouse look.
- Raycast interaction using `E`.
- Timed Evacuation and Endless Survival modes.
- Oxygen, suit health, scanner battery, medkit, battery, and supply pickups.
- Repair terminals with calibration and hold-to-stabilise interaction.
- Locked doors, animated door opening, and key/repair gated access.
- Radiation and hazard zones that increase oxygen drain.
- Security robot patrol, detection, pursuit, attack, and line-of-sight behaviour.
- Scanner mode for finding nearby interactables and robots.
- Main menu, mode selection, pause menu, options, how-to-play, credits, HUD, briefing, and result screens.
- Background music, footsteps, interaction audio, robot alerts, warning sounds, and result sounds.
- High-score and best-record persistence.
- Organised Unity hierarchy for clearer assessment and debugging.

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

## How to Run

The Unity project is located in:

```text
DeepSpaceStation_LastEvacuation_Unity/
```

Use Unity `2022.3 LTS`; the project was last verified with `2022.3.62f3c1`.

For detailed setup and play instructions, see:

- [`DeepSpaceStation_LastEvacuation_Unity/README.md`](DeepSpaceStation_LastEvacuation_Unity/README.md)
- [`DeepSpaceStation_LastEvacuation_Unity/UNITY_RUN.md`](DeepSpaceStation_LastEvacuation_Unity/UNITY_RUN.md)

## Testing and Iteration Evidence

Testing, build verification, manual playtest records, tutor feedback response, and improvement history are recorded in:

- [`DeepSpaceStation_LastEvacuation_Unity/TESTING_AND_ITERATION.md`](DeepSpaceStation_LastEvacuation_Unity/TESTING_AND_ITERATION.md)

This document records door animation tests, mode selection, pause/settings persistence, result records, robot behaviour, oxygen/hazard failure, hierarchy organisation, final build verification, and recent UI/audio polish.

## Legal, Ethical, Accessibility, and Security Considerations

External assets are credited and separated from work created for this project. Imported environment packs, fonts, TextMesh Pro resources, skyboxes, and music are documented in:

- [`DeepSpaceStation_LastEvacuation_Unity/THIRD_PARTY_ASSETS.md`](DeepSpaceStation_LastEvacuation_Unity/THIRD_PARTY_ASSETS.md)

The project does not claim third-party models, materials, fonts, or music as original student-created content. The gameplay code, interaction systems, UI integration, scene assembly, testing records, and design iteration are the main original coursework contribution.

Accessibility and usability considerations include clear HUD feedback, readable prompts, explicit objectives, failure reason text, master volume control, mouse sensitivity control, pause access, scanner feedback, warning colours, and audio cues.

The project does not use networking, online accounts, telemetry, or user data collection. Persistent settings and records are stored locally with Unity `PlayerPrefs`.

## Project Planning

The Kanban board is managed in GitHub Projects:

- [Games-Programming Project Board](https://github.com/orgs/Zeyuan-Huang-GamesProgramming/projects/1)

The board records completed work, current tasks, review items, next tasks, and backlog/stretch goals for assessment.
