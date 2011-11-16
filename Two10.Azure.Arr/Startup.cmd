webfarm_amd64_en-US.msi /quiet
requestRouter_amd64_en-US.msi /quiet

REM Settings for the site
SET ID=20
SET NAME="Default Web Site"
SET PORT=80
SET PHYSICAL_PATH=d:\inetpub\wwwroot
SET APP_POOL="ASP.NET v4.0"

REM Discover the IP Address
IPCONFIG |FIND "IPv4 Address" > %temp%\TEMPIP.txt
FOR /F "tokens=2 delims=:" %%a in (%temp%\TEMPIP.txt) do set IP=%%a
del %temp%\TEMPIP.txt
set IP=%IP:~1%

REM Configure IIS
%systemroot%\system32\inetsrv\appcmd add site /name:%NAME% /id:%ID% /bindings:http/%IP%:%PORT%: /physicalPath:%PHYSICAL_PATH%
%systemroot%\system32\inetsrv\appcmd set site /site.name:%NAME% /[path='/'].applicationPool:%APP_POOL%