PoeHud
======

Reads data from Path of Exile client application and displays it on transparent overlay, while you play PoE.

### Donation link
https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=FR956N987FHLL&lc=RU&item_name=Development%20of%20PoeHUD&item_number=OwnedCore_1&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted

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
Portal Scroll
Iron Ring
```
