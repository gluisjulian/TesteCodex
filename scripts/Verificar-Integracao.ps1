param([string]$BaseUrl = 'http://localhost:5080')
$ErrorActionPreference = 'Stop'
function Request($method, $path, $body = $null) {
    $params = @{ Uri = "$BaseUrl$path"; Method = $method }
    if ($null -ne $body) { $params.Body = ConvertTo-Json $body -Depth 8; $params.ContentType = 'application/json' }
    Invoke-RestMethod @params
}
function Check($ok, $message) { if (-not $ok) { throw $message } }
function Reject($method, $path, $body, $status) {
    try { Request $method $path $body | Out-Null } catch {
        if ([int]$_.Exception.Response.StatusCode -eq $status) { return }
        throw
    }
    throw "Esperado HTTP $status"
}
$suffix = [guid]::NewGuid().ToString('N').Substring(0,8)
$f = Request POST '/api/fornecedores' @{ razaoSocial = "Fornecedor teste $suffix" }
$i = Request POST '/api/insumos' @{ nome = "Álcool teste $suffix"; tipoInsumo = 'MateriaPrima'; unidadeMedida = 'ML' }
$path = "/api/fornecedores/$($f.id)/produtos"
$offer = @{ insumoId = $i.id; quantidadeEmbalagem = 1; unidadeEmbalagem = 'L'; precoInicial = 25 }
$p = Request POST $path $offer
Check ($p.quantidadeBase -eq 1000 -and $p.custoUnidadeBase -eq 0.025) 'Custo normalizado incorreto'
Reject POST $path @{ insumoId = $i.id; quantidadeEmbalagem = 1000; unidadeEmbalagem = 'ML'; precoInicial = 25 } 409
Request POST "/api/fornecedor-produtos/$($p.id)/precos" @{ preco = 27.90 } | Out-Null
$history = Request GET "/api/fornecedor-produtos/$($p.id)/precos"
Check ($history.total -eq 2) 'Histórico deve preservar ambos os registros'
Check (@($history.itens | Where-Object { $null -eq $_.dataFim }).Count -eq 1) 'Deve haver um vigente'
$previous = $history.itens | Where-Object { $null -ne $_.dataFim }
$current = $history.itens | Where-Object { $null -eq $_.dataFim }
Check ($previous.dataFim -eq $current.dataInicio) 'Intervalos devem ser contíguos'
Reject POST "/api/fornecedor-produtos/$($p.id)/precos" @{ preco = 27.901 } 400
Reject POST $path @{ insumoId = $i.id; quantidadeEmbalagem = 2; unidadeEmbalagem = 'L'; precoInicial = 0 } 400
$list = Request GET $path
Check ($list.total -eq 1) 'Oferta inválida não pode deixar registro persistido'
Reject PUT "/api/insumos/$($i.id)" @{ nome = "Álcool teste $suffix"; tipoInsumo = 'MateriaPrima'; unidadeMedida = 'G' } 409
Request PATCH "/api/fornecedores/$($f.id)/status" @{ ativo = $false } | Out-Null
Reject POST "/api/fornecedor-produtos/$($p.id)/precos" @{ preco = 30 } 409
$kept = Request GET "/api/fornecedor-produtos/$($p.id)/precos"
Check ($kept.total -eq 2) 'Desativação deve preservar histórico'
Write-Host 'Integração passou: cadastros, custo, duplicação, histórico, validação e desativação.'
Write-Host "Dados de teste preservados no fornecedor $($f.id); use somente banco de teste."

