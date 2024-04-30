#!/bin/bash

WORKSPACE=../..
LUBAN_DLL=$WORKSPACE/Tools/Luban/LubanLib/Luban.dll
CONF_ROOT=$WORKSPACE/Unity/Assets/Config/Excel/Datas

#Client
dotnet $LUBAN_DLL \
    -t Client \
	-f \
    --conf %CONF_ROOT%\__luban__.conf \
    -x lineEnding=CRLF 


#Server
dotnet $LUBAN_DLL \
    -t Server \
	-f \
    --conf %CONF_ROOT%\__luban__.conf \
    -x lineEnding=CRLF 


#StartConfig Release
dotnet $LUBAN_DLL \
    -t Release \
	-f \
    --conf %CONF_ROOT%\StartConfig\__luban__.conf \
    -x lineEnding=CRLF 


#StartConfig Benchmark
dotnet $LUBAN_DLL \
    -t Benchmark \
	-f \
    --conf %CONF_ROOT%\StartConfig\__luban__.conf \
    -x lineEnding=CRLF 


#StartConfig Localhost
dotnet $LUBAN_DLL \
    -t Localhost \
	-f \
    --conf %CONF_ROOT%\StartConfig\__luban__.conf \
    -x lineEnding=CRLF 


#StartConfig RouterTest
dotnet $LUBAN_DLL \
    -t RouterTest \
	-f \
    --conf %CONF_ROOT%\StartConfig\__luban__.conf \
    -x lineEnding=CRLF 

