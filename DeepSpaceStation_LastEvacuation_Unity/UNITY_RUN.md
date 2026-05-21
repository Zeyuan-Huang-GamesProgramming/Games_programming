# Unity Run Guide: Deep Space Station: Last Evacuation

## 1. Open the Project

1. Open this project folder with Unity Hub.
2. Recommended Unity version: `2022.3 LTS`.
3. On the first launch, wait for Unity to import `Packages/manifest.json` and `Assets`.

## 2. Build the Playable Scene

After Unity opens, use the top menu:

```text
Deep Space Station -> Build Playable Scene
```

Unity will create or update:

- `Assets/Scenes/Station_A.unity`
- Scene materials
- Player, camera, and HUD
- Three repair terminals
- Access and escape doors
- Oxygen canisters
- Radiation hazard zone
- Escape pod
- Objective markers, lights, props, and world-space labels

## 3. Play

1. Open `Assets/Scenes/Station_A.unity`.
2. Press Play.

## 4. Controls

- `WASD`: Move
- `Mouse`: Look around
- `Left Shift`: Sprint
- `E`: Interact
- `Esc`: Release the mouse cursor
- `R`: Restart after winning or losing

## 5. Game Objective

Before oxygen runs out and the evacuation window closes, the player must:

1. Repair `Life Support`.
2. Repair `Navigation`.
3. Repair `Reactor`.
4. Return to the escape pod and launch.

## 6. Completed Features

- First-person controller
- Oxygen drain and oxygen canister refill
- Radiation zone with faster oxygen drain
- Raycast interaction system
- Terminal repair system
- Evacuation door unlock after all repairs are complete
- Victory and failure states
- HUD with oxygen, timer, repair progress, objective text, prompts, and end screens
- Editor scene builder for generating a complete playable level
