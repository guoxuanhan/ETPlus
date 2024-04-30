#!/bin/bash

WORKSPACE=../..
LUBAN_DLL=$WORKSPACE/Tools/Luban/LubanLib/Luban.dll
CONF_ROOT=$WORKSPACE/Unity/Assets/Config/Excel/Datas
OUTPUT_CODE_DIR=$WORKSPACE/Unity/Assets/Scripts/Codes/Model/Generate
OUTPUT_DATA_DIR=$WORKSPACE/Config/Excel
OUTPUT_JSON_DIR=$WORKSPACE/Config/Json

# Client
dotnet $LUBAN_DLL \
    --customTemplateDir CustomTemplate \
    -t Client \
    -c cs-bin \
    -d bin \
    -d json \
    --conf $CONF_ROOT/__luban__.conf \
    -x outputCodeDir=$OUTPUT_CODE_DIR/Client/Config \
    -x bin.outputDataDir=$OUTPUT_DATA_DIR/c \
    -x json.outputDataDir=$OUTPUT_JSON_DIR/c \
    -x lineEnding=CRLF 

echo ==================== FuncConfig : GenClientFinish ====================

if [ $? -ne 0 ]; then
    echo "An error occurred, press any key to exit."
    read -n 1 -s
    exit 1
fi

# Server
dotnet $LUBAN_DLL \
    --customTemplateDir CustomTemplate \
    -t All \
    -c cs-bin \
    -d bin \
    -d json \
    --conf $CONF_ROOT/__luban__.conf \
    -x outputCodeDir=$OUTPUT_CODE_DIR/Server/Config \
    -x bin.outputDataDir=$OUTPUT_DATA_DIR/s \
    -x json.outputDataDir=$OUTPUT_JSON_DIR/s \
    -x lineEnding=CRLF 

echo ==================== FuncConfig : GenServerFinish ====================

if [ $? -ne 0 ]; then
    echo "An error occurred, press any key to exit."
    read -n 1 -s
    exit 1
fi

# StartConfig Release
dotnet $LUBAN_DLL \
    --customTemplateDir CustomTemplate \
    -t Release \
    -c cs-bin \
    -d bin \
    -d json \
    --conf $CONF_ROOT/StartConfig/__luban__.conf \
    -x outputCodeDir=$OUTPUT_CODE_DIR/Server/Config/StartConfig \
    -x bin.outputDataDir=$OUTPUT_DATA_DIR/s/StartConfig/Release \
    -x json.outputDataDir=$OUTPUT_JSON_DIR/s/StartConfig/Release \
    -x lineEnding=CRLF 

echo ==================== StartConfig : GenReleaseFinish ====================

if [ $? -ne 0 ]; then
    echo "An error occurred, press any key to exit."
    read -n 1 -s
    exit 1
fi

# StartConfig Benchmark
dotnet $LUBAN_DLL \
    --customTemplateDir CustomTemplate \
    -t Benchmark \
    -d bin \
    -d json \
    --conf $CONF_ROOT/StartConfig/__luban__.conf \
    -x bin.outputDataDir=$OUTPUT_DATA_DIR/s/StartConfig/Benchmark \
    -x json.outputDataDir=$OUTPUT_JSON_DIR/s/StartConfig/Benchmark

echo ==================== StartConfig : GenBenchmarkFinish ====================

if [ $? -ne 0 ]; then
    echo "An error occurred, press any key to exit."
    read -n 1 -s
    exit 1
fi

# StartConfig Localhost
dotnet $LUBAN_DLL \
    --customTemplateDir CustomTemplate \
    -t Localhost \
    -d bin \
    -d json \
    --conf $CONF_ROOT/StartConfig/__luban__.conf \
    -x bin.outputDataDir=$OUTPUT_DATA_DIR/s/StartConfig/Localhost \
    -x json.outputDataDir=$OUTPUT_JSON_DIR/s/StartConfig/Localhost

echo ==================== StartConfig : GenLocalhostFinish ====================

if [ $? -ne 0 ]; then
    echo "An error occurred, press any key to exit."
    read -n 1 -s
    exit 1
fi

# StartConfig RouterTest
dotnet $LUBAN_DLL \
    --customTemplateDir CustomTemplate \
    -t RouterTest \
    -d bin \
    -d json \
    --conf $CONF_ROOT/StartConfig/__luban__.conf \
    -x bin.outputDataDir=$OUTPUT_DATA_DIR/s/StartConfig/RouterTest \
    -x json.outputDataDir=$OUTPUT_JSON_DIR/s/StartConfig/RouterTest

echo ==================== StartConfig : GenRouterTestFinish ====================

if [ $? -ne 0 ]; then
    echo "An error occurred, press any key to exit."
    read -n 1 -s
    exit 1
fi

