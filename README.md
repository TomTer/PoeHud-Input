PoeHud
======

Reads data from Path of Exile client application and displays it on transparent overlay, while you play PoE.


### Requirements
* .NET framerwork v.4 or newer (you already have it on Windows 8+, otherwise here's your dowload link)
* Slim DX ([download link](http://slimdx.org/download.php) - pick version 4.0, for x86 platform) **Do not install x64 version**
* Windows Vista or newer (XP won't work)
* Path of Exile should be running in Windowed or Windowed Fullscreen mode (the pure Fullscreen mode does not let PoeHUD draw anything over the game window)
* Windows Aero transparency effects must be enabled. (If you get a black screen this is the issue)

### Item alert settings
The file config/crafting_bases.txt has the following syntax:
`Name,[Level],[Quality],[Rarity1,[Rarity2,[Rarity3]]]`

Examples of valid declarations:
```
Vaal Regalia,78
Corsair Sword,78,10
Gold Ring,75,,White,Rare
Ironscale Gauntlets,,10,White,Magic
Quicksilver Flask,1,5
Portal Scroll,1
Iron Ring, 1
```

### Features to-do
* Doing manually something instead of a player. Ex: call /remaining or change zones until it finds desired Strongboxes, Corrupted Zones or Masters. Configurable in settings
* "x monsters left" in xph display.
* Icons to the large map.
* Aura reminder. Ex: reminds that aura is turned off after relog.
* Outlines around the name of an item on the ground (for 6l, RBG linked etc.). Configurable in settings