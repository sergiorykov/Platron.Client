set RELEASEVERSION=%1

call restore.cmd

rem as TeamCity set
set BUILD_NUMBER=%RELEASEVERSION%

set BuildVersion=%RELEASEVERSION%
msbuild root.msbuild

set PackageVersion=%RELEASEVERSION%
msbuild nuget.msbuild