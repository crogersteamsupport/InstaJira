:: gets current directory
set myVar=%~dp0

:: tool used to sign 
set signTool="jarsigner"

:: tool used to convert pvk, password, spc files into one pfx which is already converted here to file named mypfxfile.pfx
:: set pvkTool="C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\pvk2pfx.exe"

:: command to get the pfx file is pvk2pfx.exe -pvk vio_integration.pvk -pi happiness1 -spc vio_integration_spc.spc -pfx vio-csc.pfx -f
:: the pfx file is 
set pfxFile="%myVar%teamsupport-pfx.pfx"

:: password
set password="Muroc2008!"

:: relative locations added to current directory
set fileStudioLocation="%myVar%"

echo "Working"
pause
for /r %%i in (*) do(

%signTool% -storetype pkcs12 -storepass %password% -keystore  %pfxFile% %fileStudioLocation%%%i "35c24e31-edf6-4a02-8a55-3acea403369a"

)
