[![Build status](https://ci.appveyor.com/api/projects/status/p6reklem37iyf0on?svg=true)](https://ci.appveyor.com/project/HyperGabe/discord-data-package-parser)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/HyperGabe/DiscordBot-SCP-079/blob/master/LICENSE)
# Discord Data Package Parser
An application that can parse through the discord data package onto a GUI to make it easier to read. [Learn more](https://support.discordapp.com/hc/en-us/articles/360004957991-Your-Discord-Data-Package)

## Overview
This is an application that I have made using the WPF application with the [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework-runtime)

## How to use
Using this application should be simple.  
Start by opening the data package folder using the menu strip on the top, after that everything else is done.  

You can open messages from a channel by double clicking the channel and you can copy messages by double clicking the message

# Stuff to know
* You will notice some channels called "null", these are channels that you have left/deleted/been kicked from. These names are not saved so it can not be displayed.


## Build With
* [Visual Studio Community 2017](https://visualstudio.microsoft.com/) - The IDE used to make the application
* [Json.NET](https://www.newtonsoft.com/json) - Library used to handle JSON files
* [CsvHelper](https://joshclose.github.io/CsvHelper/) - Library used to handle CsV files
* My bare hands ÒwÓ

## Author
* Gabriel FM - Initial work - [HyperGabe](https://github.com/HyperGabe)
