param(
	[Parameter(Mandatory = $true)][string]$Version,
	[Parameter(Mandatory = $true)][string]$BotToken,
	[Parameter(Mandatory = $true)][string]$ChatId
)

$dirBuild = Join-Path $env:TEMP "RetailCorrectorBuild"
dotnet publish --sc -r win-x86 -o $dirBuild -c Release -p:Version=$Version

$outFile = Join-Path $env:TEMP ("RetailCorrector-{0}.exe" -f $Version)
$issPath = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
& $issPath "/DAppVersion=$Version" "/DSourcePath=$dirBuild" "/DOutDir=$env:TEMP" "RetailCorrector.iss"

dotnet clean

$tgUrl = "https://api.telegram.org/bot$BotToken/sendDocument"

$resp = Invoke-WebRequest -Method Post -Uri $tgUrl -Form @{
  chat_id = $ChatId
  document = Get-Item $outFile
}

Remove-Item -Recurse -Force $dirBuild -ErrorAction SilentlyContinue
Remove-Item -Force $outFile -ErrorAction SilentlyContinue

$postId = $resp.result.message_id
$tgMessageUrl = "https://t.me/RetailCorrector/$postId"
$releaseUrl = "https://gitlab.com/api/v4/projects/71864399/releases/$Version/assets/links"
Invoke-RestMethod -Method Post -Uri $releaseUrl `
        -Headers @{ "PRIVATE-TOKEN" = $env:GITLAB_CI_TOKEN } `
        -ContentType "application/json" `
        -Body (@{ 
          name = "Пост для скачивания"
          url = $tgMessageUrl 
        } | ConvertTo-Json -Depth 3)