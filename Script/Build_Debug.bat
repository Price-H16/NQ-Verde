rd /s /q Debug
mkdir Debug

xcopy /y /e /s /i ..\bin\Debug\Parser Debug\Parser
xcopy /y /e /s /i ..\bin\Debug\Login Debug\Login
xcopy /y /e /s /i ..\bin\Debug\Master Debug\Master
xcopy /y /e /s /i ..\bin\Debug\World Debug\World

::xcopy /y /e /s /i Config\CH1 Debug\OpenNos.World\CH1
::xcopy /y /e /s /i OpenNos.World\bin\Debug Debug\OpenNos.World\CH51 /exclude:exclude.txt

copy Run_All.bat Debug\Run.bat