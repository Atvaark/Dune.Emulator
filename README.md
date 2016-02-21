#Dune.Emulator
Dune.Emulator is C# .NET library that implements the network protocol used in Capcom's 2012 action role-playing game *Dragon's Dogma*.

##Requirements
* Visual Studio 2013
* .NET Framework 4.5

##Projects

###Dune
A class library which contains the packet definitions and network protocol implementation.

###Dune.Client
A console application which displays the current official online Ur-Dragon status.
To run this client you initially need a running steam client and a steam profile which owns Dragon's Dogma: Dark Arisen. Once authenticated successfully, the client will cache the session in *credentials.txt*.

###Dune.Server
A console application which hosts an online Ur-Dragon server.

To run this server you need to create an X509 certificate, rename it *dune.pfx* and place it in the same directory as *Dune.Server.exe*.
In order to connect to this server you can use the mod [dinput8.dll hook](http://www.nexusmods.com/dragonsdogma/mods/96/?), hex edit the game executable or use and configure the client project.

###Dune.Proxy
A console application which acts as a proxy between a client and the official server.

To run this proxy you also require an X509 certificate. [See Dune.Server for instructions.](#duneserver)