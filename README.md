# TcPlayer2

TC Player 2 is a ground up recreation of my previous player, TCPlayer (https://github.com/webmaster442/TCPlayer)

## Why a rewite?

When I developed TCPlayer, it started a small project and back then I didn't gave much thought into designing it for the future, so over the time adding new features to it became hard and some design decisions that I made back then, made the code unupgradable without a massive rewite.

## Features 

Some features were ditched from the original project. These include:

* Localization support
* Support for tracker files
* Support for Windows7/8
* Support for 32 bit operating systems.

New features:

* Windows 10 itegration
* Support for loading and streaming from DLNA servers
* Support for loading and playing youtube streams (for this a copy of youtube-dl is required)
* Web based remote support
* 5 band equalizer

## Development

The project currently uses .NET 5 and requires VS 2019 as a development tool.
