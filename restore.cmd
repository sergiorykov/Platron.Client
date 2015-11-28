@echo off

rem Partially based on ideas from https://github.com/loresoft/KickStart

md Source\packages
Tools\nuget.exe restore Source\Platron.sln -PackagesDirectory Source\packages

Tools\nuget.exe install Tools\packages.config -OutputDirectory Tools -ExcludeVersion -NonInteractive