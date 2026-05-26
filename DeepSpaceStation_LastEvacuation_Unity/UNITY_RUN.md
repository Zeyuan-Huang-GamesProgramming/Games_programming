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
- Scene materials, lighting, and environment details
- Player, camera, and HUD
- Repair terminals and access doors
- Resource and key-item pickups
- Hazard zones and security robots
- Escape pod and objective markers

## 3. Play

1. Open `Assets/Scenes/Station_A.unity`.
2. Press Play.

## 4. Controls

- `WASD`: Move
- `Mouse`: Look around
- `Left Shift`: Sprint
- `E`: Interact
- `Q`: Scanner mode
- `Esc`: Release the mouse cursor
- `R`: Restart after winning or losing

## 5. Game Objective

Before oxygen runs out and the evacuation window closes, the player must:

1. Explore the station and collect required access items.
2. Repair the damaged station systems.
3. Avoid hazards and security robots while reaching the escape route.
4. Launch the escape pod.

## 6. Implemented Features

- First-person controller and raycast interactions
- Oxygen, health, scanner battery, and refill pickups
- Terminal calibration and stabilization repair gameplay
- Item-gated and repair-gated doors
- Radiation and vent hazards
- Security robot patrol, detection, and damage
- Victory and failure states
- HUD with resources, objectives, prompts, and end screens
- Editor scene builder for generating a playable level
