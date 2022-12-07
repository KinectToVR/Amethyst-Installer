@echo off
call "%~dp0..\..\..\fxc.bat" /Tvs_5_0 /Evert /Fosimple_vert.cso simple.hlsl
call "%~dp0..\..\..\fxc.bat" /Tps_5_0 /Efrag /Fosimple_frag.cso simple.hlsl

REM Move to destination
if not exist "%~dp0..\..\Resources\Shaders" mkdir "%~dp0..\..\Resources\Shaders"
move /y "%~dp0\*.cso" "%~dp0..\..\Resources\Shaders"