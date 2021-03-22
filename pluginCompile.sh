#!/bin/bash
g++ -DUNICODE -shared TotalCommander.Plugins/ListerPlugin.cpp -o bin/publish/Lister.wlx64 -lShlwapi
g++ -DUNICODE -shared TotalCommander.Plugins/PackerPlugin.cpp -o bin/publish/Packer.wcx64 -lShlwapi
