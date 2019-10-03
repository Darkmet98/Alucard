![Alucard](https://github.com/Darkmet98/Alucard/blob/master/AlucardBanner.png?raw=true)
# Alucard [![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
A simple tool to translate Gust games.

# Changelog

## 1.0
* Initial version

## 1.1
* Ported to Net Core 3
* Added the xml.e decrypt function from [SiA](https://github.com/pleonex/SiA) (Thanks @Pleonex)
* Added the function to patch the Nights Of Azure executable to load all xml.e displayeds (Thanks @Kaplas80)
* New name

# Usage
Alucard <-export/-export_folder/-import/-import_folder/-export_XML/-export_XMLFolder/-PatchExe> file/folder <NOA/NOA2/BR/ATSO>

* Export to POT example: Alucard -export EVENT_MESSAGE_MM00_OP1_010.ebm NOA
* Export folder to POT example: Alucard -export_folder MM02_CP02 NOA2
* Import PO example: Alucard -import EVENT_MESSAGE_MM00_OP1_010.po NOA
* Import folder to Po example: Alucard -import_folder MM02_CP02 NOA2
* Export XML.e example: Alucard -export_XML AbilityData.xml.e
* Export XML.e folder example: Alucard -export_XMLFolder Saves
* Patch Nights Of Azure Executable to load decrypt xml.e: Alucard -PatchExe CNN.exe

# Dictionary - Font
If you need to replace some strings (like & to á), create a "FONT_%GAME%.map" (Replace %GAME% with the name game will you translate (NOA/NOA2/BR/ATSO)) file on the program folder and put "Value original"="Value replaced" like this (&=á) and Alucard will replace the strings.

![Dictionary](https://github.com/Darkmet98/Alucard/blob/master/Tutorial1.jpg?raw=true)

# Dictionary - Character names
With Alucard, you have the posibility to get all character names for the translation and make it so easier!
First, export a file and open it with Poedit.
And you have this

![Tutorial](https://github.com/Darkmet98/Alucard/blob/master/Tutorial2.jpg?raw=true)

Do you see the hex value? Now, go to the game and reproduce the game situation with the text displayed

![Game](https://github.com/Darkmet98/Alucard/blob/master/Tutorial3.jpg?raw=true)

And now, create a "%GAME%.map" (Replace %GAME% with the name game will you import (NOA/NOA2/BR/ATSO)) file on the program folder and put "Hex value"="Name" like this (00=Sophie) and Alucard when you are exporting the files, you now have the characters names on the translation!

![Dictionary](https://github.com/Darkmet98/Alucard/blob/master/Tutorial4.jpg?raw=true)

![Dictionary](https://github.com/Darkmet98/Alucard/blob/master/Tutorial5.jpg?raw=true)

# Tested on
* Nights Of Azure
* Nights Of Azure 2
* Blue Reflection
* Atelier Sophie The Alchemist of the Mysterious Book

# Credits
* Pleonex for Yarhl libraries and SiA.
* Nex for the Yarhl node implementation.
* Kaplas80 for disable the xml encryption on NOA executable.
