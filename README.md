# L4DStatsApi
Restfulness for L4D Custom Player Stats.

# Requirements for game server
* SourceMod and MetaMod (https://www.sourcemod.com).
* SourceMod extension: REST in Pawn - Communicate with JSON REST APIs (https://forums.alliedmods.net/showthread.php?t=298024).

# Install instructions
* Make sure requirements are met.
* Either compile the plugin yourself or use the precompiled **l4dstatsapi.smx**. Copy the smx file to you sourcemod plugins folder.
* Copy the ripext folder **ca-bundle.crt** to your sourcemod ripext folder (overwrite the existing file).
* Configure the sourcemod plugin. You will need to enter at least a **GameServerGroupPrivateKey** and a **GameServerPrivateKey**.

# Requirements for hosting the REST API
* Any system that can run ASP.NET Core 2.1.

# Development
SourceMod plugin is editable with any text editor of your liking. The REST API requires Visual Studio 2017. Not sure if Visual Code supports it though?

# Notes
My plan is to first get stats recorded from player kills and deaths. This means the plugin should work on any Source engine game (supported by SourceMod). Once I get it that far, I will branch the sourcecodes and start working on L4D specific stats. Maybe CS:GO first though, since it is a lot easier to record stats from.
