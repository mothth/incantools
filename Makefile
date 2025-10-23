build:
	dotnet build proj/incantools.sln /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary /p:Configuration=Debug /p:Platform="Any CPU"
	cp "proj/bin/Debug/net48/incantools.dll" "mod/plugins/incantools.dll"
	cp "proj/bin/Debug/net48/incantools.pdb" "mod/plugins/incantools.pdb"

build-release:
	dotnet build proj/incantools.sln /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary /p:Configuration=Release /p:Platform="Any CPU"
	cp "proj/bin/Release/net48/incantools.dll" "mod/plugins/incantools.dll"
	cp "proj/bin/Release/net48/incantools.pdb" "mod/plugins/incantools.pdb"
