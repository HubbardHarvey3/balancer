# Set PowerShell as the shell for this Makefile
SHELL := C:\Program Files\Git\git-bash.exe

dir:
	echo "$(CURDIR)"

test:
	dotnet test --configuration Debug --settings coverlet.runsettings
	reportgenerator "-reports:Balancer.Tests/TestResults/**/coverage.cobertura.xml" "-targetdir:coveragereport" "-reporttypes:Html"


build:
	dotnet publish Balancer/Balancer.csproj -f net9.0-windows10.0.19041.0 -c Release -p:WindowsPackageType=MSIX
