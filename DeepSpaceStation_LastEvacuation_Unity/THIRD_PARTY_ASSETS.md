# Third-Party Assets and Attribution

## Project

**Deep Space Station: Last Evacuation** is a Unity first-person science-fiction
survival and evacuation game. This document records third-party resources used
in the playable vertical slice and distinguishes them from work created for the
project.

This record is provided for assessment, transparency, and responsible release
management. It is not a replacement for the licence terms supplied by each
publisher or by the Unity Asset Store.

## Important Distribution Notice

Some assets in this project were downloaded as free Unity resources. "Free" does
not automatically mean that the original asset source files may be republished
in a public GitHub repository.

In particular, the locally retained `Assets/Free Skyboxes - Space/Readme.txt`
identifies Dogmatic as the copyright holder and states that redistribution of
the skybox pack is not permitted. The raw files for that pack must therefore not
be published in a public source repository unless separate permission is
obtained.

Before making this repository public, each third-party package should be checked
against its current publisher licence and the
[Unity Asset Store Terms of Service and EULA](https://unity.com/legal/as-terms).
Where source redistribution is not authorised, the repository should contain
this credits record and project code, while assessors should use an authorised
local import or a compiled playable build.

## Third-Party Resources Used

| Resource | Use in the game | Project location | Source / attribution and release note |
| --- | --- | --- | --- |
| **SCI-FI Styled Modular Pack** | Modular corridor architecture, door and frame visuals, window sections, lights, storage boxes, and station surface materials. | `Assets/Sci Fi Modular Pack/` | Downloaded as a free Unity Asset Store resource. The source recorded in the project notes is [SCI-FI Styled Modular Pack](https://assetstore.unity.com/packages/3d/environments/sci-fi/sci-fi-styled-modular-pack-82913). Original models and materials remain the work of their publisher. Do not redistribute the raw pack unless its licence permits this. |
| **Cosmic Retro Station Props FREE** | Environmental storytelling props including computer panels, monitors, storage crates, emergency lockers, table, chair, and wall control panels. | `Assets/Cosmic_Retro_Station_Props_FREE/` | Imported free third-party pack; its supplied `Readme.pdf` is retained in the local project. The exact download-page URL and publisher details should be copied from the original download record before final public submission. Original props are not claimed as student-created work. |
| **Free Skyboxes - Space** | Background space environment; the playable scene uses the `SBS Space 1/Large` skybox material. | `Assets/Free Skyboxes - Space/` | Copyright Dogmatic, all rights reserved, as identified by the supplied README. That README explicitly prohibits redistribution. The source files must be excluded from a public GitHub release unless permission is granted. |
| **Teko Bold typeface asset** | Display font used for the science-fiction HUD and TextMesh Pro UI styling; a TextMesh Pro SDF font asset is generated from it. | `Assets/Fonts/Teko-Bold.ttf`; `Assets/Fonts/Teko-Bold SDF.asset` | The font file was sourced during development from an installed Unity package sample in this local project. A specific font licence file has not yet been retained with the imported font, so its upstream licence and attribution must be confirmed before public redistribution of the font file. |
| **TextMesh Pro Essential Resources** | Text rendering resources supporting the UI conversion and HUD presentation. | `Assets/TextMesh Pro/`; package dependency in `Packages/manifest.json` | Unity TextMesh Pro resources. Supplied third-party attributions, including the included font and sprite attribution files, should be preserved when those resources are distributed. |

## Resources Created for This Project

The following parts of the vertical slice were created or integrated specifically
for this project and are not presented as downloaded asset-pack content:

- Gameplay programming for oxygen, health, interaction, objectives, evacuation,
  scoring, records, pause settings, and player feedback.
- Scene construction and placement logic used to assemble the playable station
  from imported environment assets.
- HUD layout, opening briefing, results presentation, and settings integration.
- Runtime procedural game audio, including ambience and feedback sounds. No
  downloaded audio sample files were identified in the project asset folders.

## Evidence of Responsible Asset Use

- Third-party resources are identified by package name and by their role in the
  final scene rather than being claimed as original models or textures.
- Publisher documentation supplied with imported packages is retained locally
  for licence checking.
- The project uses imported environment assets to support a focused vertical
  slice, while original work is demonstrated through gameplay logic, UI,
  systems integration, level assembly, testing, and iteration.
- Distribution restrictions are recorded before a public source release is
  attempted.

## Public Repository Checklist

Before publishing the project repository:

1. Confirm the current licence and source URL for every downloaded package.
2. Exclude `Assets/Free Skyboxes - Space/` raw source files from any public
   repository unless separate distribution permission has been obtained.
3. Confirm whether the two imported prop/environment packs permit raw source
   redistribution; exclude them if permission is not clear.
4. Obtain and retain the upstream licence notice for `Teko-Bold.ttf`, or replace
   it with a font whose redistribution terms are recorded in the repository.
5. Keep this document, the README, build/run instructions, and screenshots or a
   playable build available for assessment.

## Reference Links

- [Unity Asset Store Terms of Service and EULA](https://unity.com/legal/as-terms)
- [SCI-FI Styled Modular Pack listing recorded in project notes](https://assetstore.unity.com/packages/3d/environments/sci-fi/sci-fi-styled-modular-pack-82913)
- [Skybox Studio product page referenced by the Free Skyboxes README](https://assetstore.unity.com/packages/tools/level-design/skybox-studio-178954)
