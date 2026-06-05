@echo off
set SERVER=tcp:schoolms2.database.windows.net,1433
set DB=SchoolManagementSystemDb
set USER=schoolmsadmin
set PASS=Admin@12345

for %%F in ("%~dp0*.sql") do (
  echo Executing %%~nF.sql ...
  sqlcmd -S %SERVER% -d %DB% -U %USER% -P "%PASS%" -i "%%F"
)

pause
