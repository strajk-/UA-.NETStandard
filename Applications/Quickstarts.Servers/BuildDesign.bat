@echo off
setlocal

REM if docker is not available, ensure the Opc.Ua.ModelCompiler.exe is in the PATH
set MODELCOMPILER=Opc.Ua.ModelCompiler.exe
set MODELCOMPILERIMAGE=ghcr.io/opcf-members/ua-modelcompiler:latest
set MODELROOT=.

echo pull latest modelcompiler from github container registry
docker login ghcr.io -u <user> -p <pw>
IF ERRORLEVEL 1 goto nodocker
docker pull %MODELCOMPILERIMAGE%
IF ERRORLEVEL 1 (
:nodocker
    Echo The docker command to download ModelCompiler failed. Using local PATH instead to execute ModelCompiler.
) ELSE (
    Echo Successfulled pulled the latest docker container for ModelCompiler.
    set MODELROOT=/model
    set MODELCOMPILER=docker run -v "%CD%:/model" -it --rm --name ua-modelcompiler %MODELCOMPILERIMAGE% 
)

echo Building TestData
%MODELCOMPILER% compile -version v104 -d2 "%MODELROOT%/TestData/TestDataDesign.xml" -cg "%MODELROOT%/TestData/TestDataDesign.csv" -o2 "%MODELROOT%/TestData"
IF %ERRORLEVEL% EQU 0 echo Success!

echo Building MemoryBuffer
%MODELCOMPILER% compile -version v104 -d2 "%MODELROOT%/MemoryBuffer/MemoryBufferDesign.xml" -cg "%MODELROOT%/MemoryBuffer/MemoryBufferDesign.csv" -o2 "%MODELROOT%/MemoryBuffer" 
IF %ERRORLEVEL% EQU 0 echo Success!

echo Building BoilerDesign
%MODELCOMPILER% compile -version v104 -d2 "%MODELROOT%/Boiler/BoilerDesign.xml" -c "%MODELROOT%/Boiler/BoilerDesign.csv" -o2 "%MODELROOT%/Boiler"
IF %ERRORLEVEL% EQU 0 echo Success!

echo Building StructuresWithArrays from Nodeset2
%MODELCOMPILER% compile -version v104 -d2 "%MODELROOT%/StructuresWithArrays.Nodeset2.xml,StructuresWithArrays,StructuresWithArrays" -o2 "%MODELROOT%/StructuresWithArrays"
IF %ERRORLEVEL% EQU 0 echo Success!
