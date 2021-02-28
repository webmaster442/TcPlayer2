#!/bin/bash
dotnet publish TcPlayer/TcPlayer.csproj -c Release -r win-x64 -p:PublishReadyToRun=true -p:PublishSingleFile=true -o bin/publish