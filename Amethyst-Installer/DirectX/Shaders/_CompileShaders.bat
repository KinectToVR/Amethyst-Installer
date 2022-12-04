@echo off
call "%~dp0..\..\..\fxc.bat" /Tvs_4_0_level_9_1 /Evert /Fosimple_vert.cso simple.hlsl
call "%~dp0..\..\..\fxc.bat" /Tps_4_0_level_9_1 /Efrag /Fosimple_frag.cso simple.hlsl

REM Move to destination
if not exist "%~dp0..\..\Resources\Shaders" mkdir "%~dp0..\..\Resources\Shaders"
move /y "%~dp0\*.cso" "%~dp0..\..\Resources\Shaders"