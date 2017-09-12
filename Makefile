#General vars
CONFIG?=Debug
ARGS:=/p:Configuration="${CONFIG}" $(ARGS)

all: provisioning
	msbuild ./System.Resources.Models.sln $(ARGS)

clean:
	msbuild ./System.Resources.Models.sln  $(ARGS) /t:Clean

package: provisioning
	msbuild ./System.Resources.Models.sln
	nuget pack ./System.Resources.Models/System.Resources.Models.csproj -properties Configuration=Release

provisioning:
	# nuget restoring
	if [ ! -f ./nuget.exe ]; then \
	    echo "nuget.exe not found! downloading latest version" ; \
	    curl -O https://dist.nuget.org/win-x86-commandline/latest/nuget.exe ; \
	fi

.PHONY: all clean pack provisioning
