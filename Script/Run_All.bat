cd %~dp0Master\
start OpenNos.Master.Server.exe
timeout 3
:: edit to have wanted amount of world servers,
:: dont forget to wait about 5 seconds before starting next world server
cd %~dp0World\
start OpenNos.World.exe
timeout 5
start OpenNos.World.exe
timeout 5
start OpenNos.World.exe
timeout 5
start OpenNos.World.exe
timeout 5
start OpenNos.World.exe
timeout 5
start OpenNos.World.exe --port 5100
timeout 5
cd %~dp0Login\
start OpenNos.Login.exe
timeout 5

exit