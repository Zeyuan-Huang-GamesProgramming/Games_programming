# Unity Run Guide: Deep Space Station: Last Evacuation

## 1. Open the Project

1. Open `DeepSpaceStation_LastEvacuation_Unity` with Unity Hub.
2. Use Unity `2022.3 LTS`; this project was last verified with
   `2022.3.62f3c1`.
3. Wait for Unity to import `Packages/manifest.json`, TextMesh Pro resources,
   and the `Assets` folder.

## 2. Open or Rebuild the Playable Scene

The playable scene is:

```text
Assets/Scenes/Station_A.unity
```

Open that scene and press Play.

If the scene needs to be regenerated, use:

```text
Deep Space Station -> Build Playable Scene
```

Additional editor tools are available from the same menu:

```text
Deep Space Station -> Apply TMP + HUD Visual Polish
Deep Space Station -> Organize Current Scene Hierarchy
```

These tools update the scene presentation and hierarchy organisation used for
the final vertical slice.

## 3. Start a Mode

When the opening screen appears:

- Press `1` for Timed Evacuation.
- Press `2` for Endless Survival.

## 4. Controls

- `WASD`: Move
- `Mouse`: Look around
- `Left Shift`: Sprint
- `C`: Crouch
- `Space`: Jump
- `E`: Interact
- `Q`: Hold scanner mode
- `Tab` / `I`: Toggle objective/inventory display
- `Esc`: Pause or resume
- `R`: Restart after winning or losing

The pause menu includes master volume and mouse sensitivity controls. These
settings are stored with `PlayerPrefs` and should persist between play sessions.

## 5. Timed Evacuation Objective

In Timed Evacuation, the player must:

1. Explore the station.
2. Repair critical systems through terminals.
3. Collect required access items.
4. Avoid hazards and security robots.
5. Reach the escape pod and launch before time or oxygen runs out.

## 6. Endless Survival Objective

In Endless Survival, the player must:

1. Survive for as long as possible.
2. Collect oxygen and supplies.
3. Avoid or escape security robots.
4. Build score while pressure increases over time.

## 7. Build Settings

`Assets/Scenes/Station_A.unity` is included in Unity Build Settings. For a final
submission build:

1. Open `File -> Build Settings`.
2. Confirm `Assets/Scenes/Station_A.unity` is listed and enabled.
3. Select the target platform, such as Windows 64-bit.
4. Build and run the exported game outside the Unity Editor.
5. Record the final build result in `TESTING_AND_ITERATION.md`.

## 8. Implemented Systems

- First-person controller and raycast interaction.
- Timed Evacuation and Endless Survival modes.
- Oxygen, health, scanner battery, pickups, hazards, and fail states.
- Terminal repair gameplay and gated doors.
- Door animation and locked-door feedback.
- Security robot patrol, detection, pursuit, and attack behaviour.
- TextMesh Pro HUD, opening screen, pause settings, and results screens.
- Runtime procedural audio feedback.
- High-score and best-record persistence.
- Organised Unity hierarchy for easier inspection and assessment.
