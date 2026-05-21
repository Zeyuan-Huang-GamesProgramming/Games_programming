# Deep Space Station: Last Evacuation

A Unity first-person sci-fi survival escape game. The player explores a damaged space station, repairs critical systems, manages oxygen and countdown pressure, unlocks the evacuation route, and escapes through the pod.

## How to Run

1. Open this folder with Unity Hub.
2. Recommended Unity version: `2022.3 LTS`.
3. Wait for Unity to import the project the first time it opens.
4. In the Unity top menu, choose `Deep Space Station -> Build Playable Scene`.
5. Open the generated scene: `Assets/Scenes/Station_A.unity`.
6. Press Play.

For more detailed setup steps, see `UNITY_RUN.md`.

## Controls

- `WASD`: Move
- `Mouse`: Look around
- `Left Shift`: Sprint
- `E`: Interact
- `Esc`: Release the mouse cursor
- `R`: Restart after winning or losing

## Current Features

- First-person movement and camera control
- Raycast interaction system
- Oxygen drain, low-oxygen failure, and oxygen canister pickup
- Countdown timer failure condition
- Three repair terminals
- Evacuation door unlock after all repairs are complete
- Radiation hazard zone that drains oxygen faster
- Escape pod victory condition
- HUD with oxygen, timer, repair progress, objectives, prompts, and end screens
- Upgraded playable station layout with lights, path guides, props, and world-space labels
- Unity Editor tool for generating the playable scene
