# L4D Statistics API
Restfulness for L4D Custom Player Stats.

# Requirements for game server
* **Valve Source engine game dedicated server**. For now any SourceMod supported game will do.
* **SourceMod and MetaMod** (https://www.sourcemod.com).
* SourceMod extension: **REST in Pawn** - Communicate with JSON REST APIs (https://forums.alliedmods.net/showthread.php?t=298024).

# Requirements for hosting the REST API
* Any system that can run **ASP.NET Core 2.1.2**.
* I am currently developing the API only for the **Microsoft SQL servers** (using the EF Core packages). Other databases shouldn't be too hard to add later on (https://docs.microsoft.com/en-us/ef/core/providers).

# Game server SourceMod plugin install instructions
* Make sure requirements are met.
* Either compile the plugin yourself or use the precompiled **l4dstatsapi.smx**. Copy the smx file to you sourcemod plugins folder.
* Copy the ripext folder **ca-bundle.crt** to your sourcemod ripext folder (overwrite the existing file).
* Configure the sourcemod plugin. You will need to enter at least a **GameServerGroupPrivateKey** and a **GameServerPrivateKey**.

# Development
SourceMod plugin is editable with any text editor of your liking. The REST API requires Visual Studio 2017. Not sure if Visual Code supports it though?

# Hosted global L4D statistics API site
You are welcome to use the L4D Statistics API I have set up here https://pilssi.dy.fi:44033/l4dstatsapi/api. You must first create a personal game server group with at least one game server to be able to authenticate to use the API. This is done by logging in using Facebook SSO. Log in to here https://pilssi.dy.fi:44033/l4dstatsapi to create your game server group and game servers.

# Roadplan
My plan is to first get stats recorded from player kills and deaths. This means the plugin should work on any Source engine game (supported by SourceMod). Once I get it that far, I will branch the sourcecodes and start working on L4D specific stats. Maybe CS:GO first though, since it is a lot easier to record stats from. I want to record pretty much everything from who shot who and what body part was hit. Player statistics can be accessed either by globally, inside a game server group or just inside a game server. Bad behaving game servers or groups can be invalidated and statistics from those places will be removed from being visible, in addition to not allowing new stats being recorded using specific API authentication(s).
