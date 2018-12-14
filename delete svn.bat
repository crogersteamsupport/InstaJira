FOR /F "tokens=*" %%G IN ('DIR /B /AD /S *_svn*') DO RMDIR /S /Q "%%G"

PAUSE