@echo off
@set unity="%Unity_Path%"
echo %unity%
echo GeneratingXcodeProj...
%unity%  -batchmode -quit -nographics -executeMethod Batchmode.BuildiOS  -logFile %~dp0Editor.log -projectPath %~dp0 
echo XcodeProjGenerated.
7z.exe a -tzip -r iosProj.zip iosProj
echo zipEnd
pause