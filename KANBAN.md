# Deep Space Station: Last Evacuation - Kanban Plan

This board tracks the development plan for the Unity coursework project. The game is already part-way through implementation, so the board records both completed work and the remaining plan from the original design document.

## Project Goal

Build a playable first-person sci-fi survival escape game set inside a failing deep-space station. The player explores connected station areas, repairs key systems, collects access items, manages oxygen and scanner battery, avoids security robots, and reaches the escape pod before the station collapses.

## Current Sprint Focus

- Improve the station layout so every room is gated by working doors and clear unlock requirements.
- Polish the visual style using imported sci-fi corridor, station prop, and space skybox assets.
- Keep the game playable from start to finish with clear mission UI, inventory, scanner, enemy pressure, and failure/win conditions.

## Kanban Board

### Done

| ID | Area | Task | Acceptance Criteria |
| --- | --- | --- | --- |
| D-01 | Project Setup | Create Unity 2022.3 project structure | Project opens in Unity 2022.3.62f3c1 and contains scenes, scripts, materials, and editor builder tools. |
| D-02 | Core Player | Implement first-person player movement | Player can walk, look around, sprint, crouch, and interact with objects. |
| D-03 | Interaction | Add E interaction system | Doors, terminals, pickups, oxygen canisters, and escape pod can respond to player interaction. |
| D-04 | Mission Flow | Add multi-stage repair objectives | Player must repair several station systems before deeper doors unlock. |
| D-05 | Map Layout | Build starting bay, central corridor, side rooms, reactor/comms/escape areas | Game has more than one room and supports staged exploration. |
| D-06 | Inventory | Add backpack and item tracking | Player can open backpack with Tab and see collected items such as keycards, fuse, decoder, medkits, and batteries. |
| D-07 | Resources | Add oxygen, health, scanner battery, medkit, and battery use | Survival stats appear in HUD and resources can be consumed or replenished. |
| D-08 | Scanner | Add Q scan mode | Scanner detects nearby interactables and security robots while draining battery. |
| D-09 | Enemy | Add security robot pressure | Robot can detect/chase/damage player and create stealth pressure. |
| D-10 | UI Foundation | Add HUD, mission panel, prompts, backpack, pause screen, and end screen basics | Player can read objectives, status, interaction prompts, and pause the game. |

### In Progress

| ID | Area | Task | Acceptance Criteria |
| --- | --- | --- | --- |
| P-01 | Visual Polish | Integrate free Asset Store sci-fi corridor, station prop, and space skybox resources | Corridor and rooms look more like a real space station instead of simple placeholder cubes. |
| P-02 | Door Logic | Fix room gates so locked rooms cannot be bypassed through visual gaps | Every locked room blocks entry until its repair count or required item condition is satisfied. |
| P-03 | Layout Polish | Align corridor modules, room openings, ceilings, and door frames | Player can move naturally without clipping, getting stuck, or seeing broken geometry. |
| P-04 | Prompt Readability | Improve world labels and HUD placement | Important lock, repair, danger, and item text is readable from normal gameplay angles. |

### To Do Next

| ID | Area | Task | Acceptance Criteria |
| --- | --- | --- | --- |
| T-01 | Stealth | Improve robot patrol routes and avoidance gameplay | Player can avoid robots by using cover, distance, and room layout rather than only running away. |
| T-02 | Challenge | Make repair tasks require simple input sequences or timing instead of only pressing E | Terminals require a short action pattern and create more active gameplay. |
| T-03 | Risk | Add more environmental hazards in corridors and rooms | Oxygen drain, radiation, blocked paths, and warning zones make route planning matter. |
| T-04 | Level Flow | Balance unlock order for Life Support, Navigation, Medical, Security, Reactor, Comms, and Escape | Player always has a clear next objective and cannot skip critical progression. |
| T-05 | Audio | Add alarm, terminal, door, pickup, scanner, and robot sound effects | Actions and danger states have readable audio feedback. |
| T-06 | Final Sequence | Improve escape pod activation and final evacuation moment | Completing all required systems lets player launch escape pod and see a clear victory result. |
| T-07 | Failure Conditions | Tune oxygen, health, robot damage, and countdown failure | Player can lose through oxygen depletion, health reaching zero, or time running out. |
| T-08 | Testing | Run full playthrough test from start to evacuation | No room traps, unreachable objectives, impossible locks, or major UI blockers remain. |

### Backlog / Stretch Goals

| ID | Area | Task | Acceptance Criteria |
| --- | --- | --- | --- |
| B-01 | Story | Add crew logs and accident background | Optional logs explain what happened on the station. |
| B-02 | Endings | Add normal, high-rank, and hidden ending conditions | Ending changes based on optional logs, remaining health/oxygen, time, and detection count. |
| B-03 | AI Variety | Add a scout drone or second enemy type | Enemy variety creates different movement/avoidance patterns. |
| B-04 | Map | Add extra maintenance room or observation deck | Optional room rewards exploration with resources or story. |
| B-05 | Presentation | Add final screenshots, controls, and short design explanation to README | Repository clearly communicates game concept, controls, and implemented features. |

## Milestones

| Milestone | Target Result | Status |
| --- | --- | --- |
| M1: Prototype | Player can move, interact, repair one system, and exit the starting area. | Done |
| M2: Playable Loop | Player explores multiple areas, completes repairs, uses inventory/resources, and avoids robot danger. | Done / In progress polish |
| M3: Visual Upgrade | Imported sci-fi assets, improved lighting, readable UI, and believable space-station rooms. | In progress |
| M4: Complete Coursework Build | Start-to-finish playable game with win/loss conditions, pause, backpack, scanner, enemy, and polished level flow. | To do |
| M5: Final Submission | README, screenshots, testing notes, and final exported build ready for assessment. | To do |

## Control Scheme

| Input | Action |
| --- | --- |
| WASD | Move |
| Mouse | Look around |
| Shift | Sprint |
| C | Crouch |
| E | Interact |
| Q | Hold scanner |
| Tab | Open backpack |
| H | Use medkit |
| B | Use battery pack |
| Esc | Pause |

## Assessment Notes

This plan is designed to show the project is more than a simple movement demo. It includes first-person control, interaction systems, staged mission progression, inventory, resource management, AI pressure, UI, imported visual assets, and final win/fail states. The remaining work is focused on polish, balancing, testing, and presentation.