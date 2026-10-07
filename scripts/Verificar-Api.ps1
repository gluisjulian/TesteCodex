param([string]$BaseUrl = 'http://localhost:5080')
$ErrorActionPreference = 'Stop'
function Check($condition, $message) { if (-not $condition) { throw $message } }
$health = Invoke-RestMethod "$BaseUrl/health"
Check ($health.status -eq 'ok') 'Health inválido'
$openapi = Invoke-RestMethod "$BaseUrl/openapi/v1.json"
Check (@($openapi.paths.PSObject.Properties).Count -ge 10) 'Endpoints OpenAPI incompletos'
Write-Host 'Health e documento OpenAPI verificados.'

