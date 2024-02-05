Build docker env
================

docker build -t jncc/sac_build_env .

Run containerised build env
===========================

docker run -it --rm -v <host/solution/folder>:/sac-microsite jncc/sac_build_env /bin/bash