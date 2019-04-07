![Pleinair](https://github.com/Darkmet98/NightsOfNewMoon/blob/master/Logo.png?raw=true)
# NightsOfNewMoon
A gust games translation toolkit.

# Usage
NightsOfNewMoon.exe <-export/-export_folder/-import/-import_folder/-credits> file/folder <NOA/NOA2/BR/ATSO>

* Export to POT example: NightsOfNewMoon,exe -export EVENT_MESSAGE_MM00_OP1_010.ebm NOA
* Export folder to POT example: NightsOfNewMoon.exe -export_folder MM02_CP02 NOA2
* Import PO example: NightsOfNewMoon.exe -import EVENT_MESSAGE_MM00_OP1_010.po NOA
* Import folder to Po example: NightsOfNewMoon.exe -import_folder MM02_CP02 NOA2

# Dictionary - Font
If you need to replace some strings (like & to á), create a "FONT_%GAME%.map" (Replace %GAME% with the name game will you translate (NOA/NOA2/BR/ATSO)) file on the program folder and put "Value original"="Value replaced" like this (&=á) and NightsOfNewMoon will replace the strings.
![Dictionary](https://github.com/Darkmet98/NightsOfNewMoon/blob/master/Tutorial1.png?raw=true)

# Dictionary - Character names
With NightsOfNewMoon, you have the posibility to get all character names for the translation and make it so easier!
First, export a file and open it with Poedit.
And you have this
![Tutorial](https://github.com/Darkmet98/NightsOfNewMoon/blob/master/Tutorial2.png?raw=true)
Do you see the hex value? Now, go to the game and reproduce the game situation with the text displayed
![Game](https://github.com/Darkmet98/NightsOfNewMoon/blob/master/Tutorial3.png?raw=true)
And now, create a "%GAME%.map" (Replace %GAME% with the name game will you import (NOA/NOA2/BR/ATSO)) file on the program folder and put "Hex value"="Name" like this (00=Sophie) and NightsOfNewMoon when you are exporting the files, you now have the characters names on the translation!
![Dictionary](https://github.com/Darkmet98/NightsOfNewMoon/blob/master/Tutorial4.png?raw=true)
![Dictionary](https://github.com/Darkmet98/NightsOfNewMoon/blob/master/Tutorial5.png?raw=true)

# Tested on
* Nights Of Azure
* Nights Of Azure 2
* Blue Reflection
* Atelier Sophie The Alchemist of the Mysterious Book

# Credits
* Thanks to Pleonex for Yarhl libraries and Nex for the Yarhl node implementation.