set WORKSPACE=..\..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\LubanLib\Luban.dll
set CONF_ROOT=%WORKSPACE%\Unity\Assets\Config\Excel\Datas
set OUTPUT_CODE_DIR=%WORKSPACE%\Unity\Assets\Scripts\Codes\Model\Generate
set OUTPUT_DATA_DIR=%WORKSPACE%\Config\Excel
set OUTPUT_JSON_DIR=%WORKSPACE%\Config\Json

::Client
dotnet %LUBAN_DLL% ^
    --customTemplateDir CustomTemplate ^
    -t Client ^
    -c cs-bin ^
    -d bin ^
    -d json ^
    --conf %CONF_ROOT%\__luban__.conf ^
    -x outputCodeDir=%OUTPUT_CODE_DIR%\Client\Config ^
    -x bin.outputDataDir=%OUTPUT_DATA_DIR%\c ^
    -x json.outputDataDir=%OUTPUT_JSON_DIR%\c ^
    -x lineEnding=CRLF 

echo ==================== FuncConfig : GenClientFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)


::Server
dotnet %LUBAN_DLL% ^
    --customTemplateDir CustomTemplate ^
    -t All ^
    -c cs-bin ^
    -d bin ^
    -d json ^
    --conf %CONF_ROOT%\__luban__.conf ^
    -x outputCodeDir=%OUTPUT_CODE_DIR%\Server\Config ^
    -x bin.outputDataDir=%OUTPUT_DATA_DIR%\s ^
    -x json.outputDataDir=%OUTPUT_JSON_DIR%\s ^
    -x lineEnding=CRLF 
    
echo ==================== FuncConfig : GenServerFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)


::StartConfig Release
dotnet %LUBAN_DLL% ^
    --customTemplateDir CustomTemplate ^
    -t Release ^
    -c cs-bin ^
    -d bin ^
    -d json ^
    --conf %CONF_ROOT%\StartConfig\__luban__.conf ^
    -x outputCodeDir=%OUTPUT_CODE_DIR%\Server\Config\StartConfig ^
    -x bin.outputDataDir=%OUTPUT_DATA_DIR%\s\StartConfig\Release ^
    -x json.outputDataDir=%OUTPUT_JSON_DIR%\s\StartConfig\Release ^
    -x lineEnding=CRLF 
    
echo ==================== StartConfig : GenReleaseFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)


::StartConfig Benchmark
dotnet %LUBAN_DLL% ^
    --customTemplateDir CustomTemplate ^
    -t Benchmark ^
    -d bin ^
    -d json ^
    --conf %CONF_ROOT%\StartConfig\__luban__.conf ^
    -x bin.outputDataDir=%OUTPUT_DATA_DIR%\s\StartConfig\Benchmark ^
    -x json.outputDataDir=%OUTPUT_JSON_DIR%\s\StartConfig\Benchmark ^
    -x lineEnding=CRLF 
    
echo ==================== StartConfig : GenBenchmarkFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)


::StartConfig Localhost
dotnet %LUBAN_DLL% ^
    --customTemplateDir CustomTemplate ^
    -t Localhost ^
    -d bin ^
    -d json ^
    --conf %CONF_ROOT%\StartConfig\__luban__.conf ^
    -x bin.outputDataDir=%OUTPUT_DATA_DIR%\s\StartConfig\Localhost ^
    -x json.outputDataDir=%OUTPUT_JSON_DIR%\s\StartConfig\Localhost ^
    -x lineEnding=CRLF 
    
echo ==================== StartConfig : GenLocalhostFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)

::StartConfig RouterTest
dotnet %LUBAN_DLL% ^
    --customTemplateDir CustomTemplate ^
    -t RouterTest ^
    -d bin ^
    -d json ^
    --conf %CONF_ROOT%\StartConfig\__luban__.conf ^
    -x bin.outputDataDir=%OUTPUT_DATA_DIR%\s\StartConfig\RouterTest ^
    -x json.outputDataDir=%OUTPUT_JSON_DIR%\s\StartConfig\RouterTest ^
    -x lineEnding=CRLF 
    
echo ==================== StartConfig : GenRouterTestFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)

