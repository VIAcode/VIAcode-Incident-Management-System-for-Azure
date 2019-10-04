param(
    [Parameter(Mandatory = $true)]
    $projectPath,
    [Parameter(Mandatory = $true)]
    $templateDir,
    [Parameter(Mandatory = $true)]
    $configuration,
    [Parameter(Mandatory = $true)]
    $definitionfileName
)

$connectorProjects = @("DashboardReport\DashboardReport",
"VIMSConnector\VIMSConnector")
$providerProjects = @("ResourceProvider\VimsAPI")

if (Test-Path $definitionfileName) { Remove-Item $definitionfileName }
if (Test-Path "providerApi.zip") { Remove-Item "providerApi.zip"}
if (Test-Path "connector.zip") { Remove-Item "connector.zip"}



Set-Location $projectPath
foreach($prj in $providerProjects)
{
    Get-ChildItem "$prj\bin\$configuration\netcoreapp2.1\" | Compress-Archive -DestinationPath "providerApi.zip" -Update
}
foreach($prj in $connectorProjects)
{
    Get-ChildItem "$prj\bin\$configuration\netcoreapp2.1\" | Compress-Archive -DestinationPath "connector.zip" -Update
}

Get-ChildItem $templateDir | Compress-Archive -DestinationPath $definitionfileName -Update
Compress-Archive "providerApi.zip", "connector.zip" -DestinationPath $definitionfileName -Update
