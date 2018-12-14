FIND /C /I "app.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				trunk.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts
FIND /C /I "portal.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				signalr.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts
FIND /C /I "portal.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				portal.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts
FIND /C /I "portal.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				mobile.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts
FIND /C /I "portal.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				m.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts
FIND /C /I "portal.tsdev.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1				signup.tsdev.com>>%WINDIR%\system32\drivers\etc\hosts

