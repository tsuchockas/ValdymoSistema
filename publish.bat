@echo off

echo ===================== 1 =====================
echo * Current working direcotry: '%CD%'
SET initialWorkingDir=%CD%
SET scriptDir=%~dp0
SET appDir=%scriptDir%\ValdymoSistema
PUSHD %~dp0

echo ===================== 2 =====================
echo * Getting dotnet info (it should be higher or equal to v3.1)...
call dotnet --info
if ERRORLEVEL 1 goto _error

echo ===================== 3 =====================
echo * Current working direcotry: '%CD%'
echo * Restoring nuget packages...
call dotnet restore
if ERRORLEVEL 1 goto _error

echo ===================== 4 =====================
PUSHD "%appDir%"
echo * Current working direcotry: '%CD%'
echo * Publishing ValdymoSistema project...
echo * Publishing 'netcoreapp3.1' EXE verison...
call dotnet publish "%scriptDir%ValdymoSistema\ValdymoSistema.csproj" -o "%scriptDir%Publish\ValdymoSistema\netcoreapp_win10-x64" -f netcoreapp3.1 -r win10-x64 -c Release


echo ===================== 5 =====================
PUSHD "%initialWorkingDir%"
echo * Set working directory back to '%CD%'
if ERRORLEVEL 1 goto _error

goto _end

:_error
PUSHD "%initialWorkingDir%"
echo ERROR occurred, exiting
exit 1

:_end
@endlocal