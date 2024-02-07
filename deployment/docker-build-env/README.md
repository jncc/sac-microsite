Build docker env
================

docker build -t jncc/sac_build_env .

Run containerised build env
===========================

docker run -u vscode -it -p 5000:5000 --rm -v <host/solution/folder>:/sac-microsite jncc/sac_build_env /bin/bash


docker run -u vscode -it -p 5000:5000 --rm -v ~/working/sac-microsite:/sac-microsite jncc/sac_build_env /bin/bash