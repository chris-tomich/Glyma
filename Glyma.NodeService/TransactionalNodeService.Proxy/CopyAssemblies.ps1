param(
  [string]$assemblyLocation,
  [string]$solutionDir
)

$assemblyDestination = $solutionDir + "..\Glyma.SharePoint2013\MappingToolSPDeployment2013\Assemblies\"

copy $assemblyLocation $assemblyDestination