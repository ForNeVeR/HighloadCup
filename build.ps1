Set-Location src
.\build.cmd
Set-Location ..
Copy-Item Microsoft.AspNetCore.Server.Kestrel.dll artifacts
docker build -t hcup/odin .
# docker login stor.highloadcup.ru
docker tag hcup/odin stor.highloadcup.ru/travels/safe_pike
docker push stor.highloadcup.ru/travels/safe_pike