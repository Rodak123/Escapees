# Escapees

<a href="https://rodakdev.itch.io/escapees" title="Escapees on Itch">
  <img width="640" height="640" alt="Escapees Cover" src="https://github.com/user-attachments/assets/23fe3895-82a8-4007-a623-7c678e9241c0" />
  Escapees on itch.io
</a>
<hr>

A game for the [LOWREZJAM 2026](https://itch.io/jam/lowrezjam-2026)

**Requirement**: 64x64 screen

**Themes (all optional)**:

- Time Is A Construct
- Save. That. For. Later. Or… Die.
- Pattern Recognition
- Biological Restrictions
- Obey The Game Master
- **Fugitive**
- Gotta Go Fast
- Seeing With Sound
- You're Not Allowed To Win - maybe I'll add this at the end? law always wins?
- Cleanup Crew

## Open Source Disclaimer

The provided MIT license is for the whole project. However since I can't redistribute the music, I have replaced all files with silence.
Other than that, this is the whole project. :D

## Story / Areas

Each area is more difficult than the last.

### Area 0: Prepares (Tutorial)

Starting area.
Only one escapee.
The player will try out his tools and the escapee will prepare the escape plan.

### Area 1: Prison [ unfinished ]

Inside the prison building.
The player frees escapees from prison cells, workout rooms and their goal is to disable the prison generator.

### Area 2: Prison Yard [ not started ]

Inside the prison area.
The player helps the escapees find their way though the convoluted prison yards and through its multiple gates (what are they in here for??)

### Area 3: On the run [ not started ]

Outside the prison completely.
The player navigates the escapees through difficult terrain.

maybe: time limited puzzles, police is chasing you down

## Unplanned - Fixes / Changes / Additions

- [x] Refactor tools
- [x] Add an "Copy Score" button to the main menu/level select that copies the solved levels and their stats to the clipboard for easy sharing (IMPORTANT)
- [x] Change stop sign to be always RMB and use the position of the mouse as direction
- [ ] Add screen edge indicators for escapees, start and exit positions (IMPORTANT)
- [ ] Add more ghost visuals that tell what will the tool do exactly (outline) 
- [ ] Add scrolling to tool picking
- [ ] Try restarting level without reloading the scene

## Planned To-Do 

- [x] Tools (art + animations + logic)
  - [x] Drill - dig forward
  - [x] Hammer - build straight bridge
  - [x] Parachute - soften fall
  - [x] Pickaxe - dig down
  - [x] Stop - stops
- [x] Game Loop
  - [x] Lebro lives / Count
  - [x] Death/Fail state - when there are not enough lebros left
  - [x] Win/Success state - when all alive lebros left
  - [x] Spawner
  - [x] Exit
  - [x] Pause menu
    - [x] See current stats
    - [x] Quick reset
    - [x] Back to levels
  - [x] Level screen
    - [x] Level selection
    - [x] Clicking level or completing shows the level stats (time to complete, % saved)
    - [x] Unlocking levels after completing minimum
  - [x] Main menu
- [ ] Features
  - [x] Picking up items (RMB)
  - [x] Camera movement (MMB or WSAD or Arrows or Screen Edges)
  - [x] Quick reset (R)
  - [x] Apply theme (Fugitive?)
    - [x] Reskin lebros to escapees
    - [x] Add prison-themed tiles
    - [x] More backgrounds
  - [ ] Levels (at least 10)
  - [x] Saving progress
- [x] Sounds
  - [x] Music (use 80 music pack)
  - [x] Tools
  - [x] Walking + Hitting ground
  - [x] Map destroying tiles
  - [x] UI
- [ ] Polish
  - [ ] Scene transitions
  - [ ] Particle effects?
  - [ ] Speed up button
- [ ] Itch Page
  - [x] Game name
  - [x] Game gif
  - [x] Description
  - [ ] Marketing images
    - [x] App icon - 256 x 256
    - [ ] Splash screen?
    - [x] Cover - 630 x 500
    - [x] Banners - 512 x 128
    - [x] Screenshots - 64x64 or rather 960x960
- [x] Testing
  - [x] Builds
    - [x] Web
    - [ ] Linux
    - [x] Windows
