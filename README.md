# RG-Tools Add-in for Autodesk Revit

A C# .NET Add-in for Autodesk Revit containing a set of tools for BIM Coordination.

## Installation

Simply run setup file. It will unpack Add-in and it should be added to Revit at the start of new session.
The installer will install all supported versions mentioned in the description.

### Update version / Reinstall
Go to "Add or remove program" on your PC and fing the RG-Tools in the list of the installed programs. Delete the app and install new version if needed.

## Features 

The interface mainly mimicks the [pyArchitect](https://github.com/romangolev/pyArchitect) extension and mainly have the same features mostly aiming to help BIM Coordinators. 
All the feautures have tooltips with description. 
###

## Versions supported
Currently supported versions:
- Autodesk Revit 2020
- Autodesk Revit 2021
- Autodesk Revit 2022
- Autodesk Revit 2023

## Localization
The Add-in is available in English, Spanish and Russian. It automatically detects version of the current Revit instance and runs a localized version. 
Revit version can be picked by specifying launch icon like [this](https://help.autodesk.com/view/RVT/2023/ENU/?guid=GUID-BD09C1B4-5520-475D-BE7E-773642EEBD6C).
The Add-in goes with localization of the interface and tooltip information (comes up when hovering cursor over the button), yet the ribbon tools names are only in English.

## Shoutouts

Big thanks to:
 - [Jeremy Tammik](https://github.com/jeremytammik) for a great job providing sample library and continoius support on the [Building Coder](https://thebuildingcoder.typepad.com/) blog.
 - [Gui Talarico](https://github.com/gtalarico) for Revit Api docs [website](https://www.revitapidocs.com/).


