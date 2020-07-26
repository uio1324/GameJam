@echo off
@set unity="%Unity_Path%"
echo %unity%
echo GeneratingAPK...
%unity%  -batchmode -quit -nographics -executeMethod Batchmode.BuildAndroid  -logFile %~dp0Editor.log -projectPath %~dp0 
echo APKGenerated.
pause