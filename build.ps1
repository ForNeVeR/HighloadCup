Set-Location src
.\build.cmd
Set-Location ..
get-childitem -path ".\kestrel" -recurse | copy-item -destination ".\artifacts"
docker build -t hcup/fornever .
# docker run -it --rm -p 80:80 hcup/odin
# docker login stor.highloadcup.ru
docker tag hcup/fornever stor.highloadcup.ru/travels/nice_toucan
docker push stor.highloadcup.ru/travels/nice_toucan