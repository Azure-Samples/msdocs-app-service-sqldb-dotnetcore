FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm

# Drop the stale Yarn apt source baked into the base image; its signing
# key (62D54FD4003F6525) is no longer accepted by Yarn, causing
# `apt-get update` to fail during devcontainer feature installs.
# The tutorial doesn't use Yarn.
RUN rm -f /etc/apt/sources.list.d/yarn.list

# Install SQL Tools: SQLPackage and sqlcmd
COPY mssql/installSQLtools.sh installSQLtools.sh
RUN bash ./installSQLtools.sh \
     && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts

# [Optional] Uncomment this section to install additional OS packages.
# RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
#     && apt-get -y install --no-install-recommends <your-package-list-here>

# [Optional] Uncomment this line to install global node packages.
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1
