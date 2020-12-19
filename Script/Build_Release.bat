rd /s /q Release
mkdir Release

xcopy /y /e /s /i ..\bin\Release\Parser Release\Parser
xcopy /y /e /s /i ..\bin\Release\Login Release\Login
xcopy /y /e /s /i ..\bin\Release\Master Release\Master
xcopy /y /e /s /i ..\bin\Release\World Release\World

::xcopy /y /e /s /i Config\CH1 Release\OpenNos.World\CH1
::xcopy /y /e /s /i OpenNos.World\bin\Release Release\OpenNos.World\CH51 /exclude:exclude.txt

copy Run_All.bat Release\Run.bat