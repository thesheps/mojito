@echo Off

set config=%1
if "%config%" == "" (
  	set config=Release
)
 
set version=1.0.0
if not "%PackageVersion%" == "" (
  	set version=%PackageVersion%
)

set nuget=
if "%nuget%" == "" (
	set nuget=nuget
)

REM Restore
call %NuGet% restore src\Mojito\packages.config -OutputDirectory %cd%\packages -NonInteractive
call %NuGet% restore tests\Mojito.Tests\packages.config -OutputDirectory %cd%\packages -NonInteractive

REM Build
msbuild Mojito.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false
if not "%errorlevel%"=="0" goto failure

REM Unit Tests
call %nuget% install NUnit.Runners -Version 2.6.4 -OutputDirectory packages
packages\NUnit.Runners.2.6.4\tools\nunit-console.exe /config:%config% /framework:net-4.5 tests\Mojito.Tests\bin\%config%\Mojito.Tests.dll
if not "%errorlevel%"=="0" goto failure

REM Package
mkdir Build
mkdir Build\lib
mkdir Build\lib\net40
call %nuget% pack "src\Mojito.nuspec" -IncludeReferencedProjects -NoPackageAnalysis -verbosity detailed -o Build -Version %version% -p Configuration="%config%"

:success
exit 0

:failure
exit -1