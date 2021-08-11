#General vars
CONFIG?=Debug
ARGS:=/r /p:Configuration="${CONFIG}" $(ARGS)

all: provisioning
	msbuild ./System.Resources.Models.sln $(ARGS)

clean:
	msbuild ./System.Resources.Models.sln  $(ARGS) /t:Clean

package:
	msbuild ./System.Resources.Models.sln
	nuget pack ./System.Resources.Models/System.Resources.Models.csproj -properties Configuration=Release

.PHONY: all clean pack
