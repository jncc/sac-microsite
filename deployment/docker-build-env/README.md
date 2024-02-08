# Build Env
This is a containerised build environment containing .NET core 2.2 sdk and a variety of dev tools. At the time of release it is compatible with VS Code dev container.

Logging on as user *vscode* ensures that files created in the host file system will be created by UID 1000, GID 1000 which are the common ID's for the default user and group. These can be changed in the container build.

Oh My ZSH is also installed for this user.

.NET core is globally available to both the root user and vscode.

Build docker env
================

docker build -t jncc/sac_build_env .

Run containerised build env
===========================

docker run -u vscode -it -p 5000:5000 --rm -v <host/solution/folder>:/sac-microsite jncc/sac_build_env /bin/bash
 hi there
 