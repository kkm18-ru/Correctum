$prep_version = (Invoke-WebRequest "https://gitlab.com/api/v4/projects/$CI_PROJECT_ID/repository/tags?order_by=updated&sort=desc" -Headers @{"PRIVATE-TOKEN" = $CI_JOB_TOKEN} | ConvertFrom-Json | Select-Object -Index 0).name
$compare = [Text.Encoding]::UTF8.GetString((Invoke-WebRequest "https://gitlab.com/api/v4/projects/$CI_PROJECT_ID/repository/compare?from=$prep_version&to=dev" -Headers @{"PRIVATE-TOKEN" = $CI_JOB_TOKEN}).RawContentStream.ToArray()) | ConvertFrom-Json

$is_major = $compare.commits.title.Where({$_ -match '^[a-zA-Z]+!:\ '}).Count -gt 0
$is_minor = $compare.commits.title.Where({$_ -match '^feat!?:\ '}).Count -gt 0
$is_patch = $compare.commits.title.Where({$_ -match '^(bug)?fix!?:\ '}).Count -gt 0

if(-not $is_major -and -not $is_minor -and -not $is_patch) { exit 1 }

$v = $prep_version.Split('.')
if($is_major) { 
    [int]$v[0] += 1
    [int]$v[1] = 0
    [int]$v[2] = 0
} elseif($is_minor){
    [int]$v[1] += 1
    [int]$v[2] = 0
} else { [int]$v[2] += 1 }
$version = $v -join '.'

dotnet publish RetailCorrector/RetailCorrector.csproj --sc -r win-x86 -o bin -c Release -p:Version=$version
& 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe' "/DAppVersion=$version" "/DSourcePath=bin" "/DOutDir=." "RetailCorrector.iss"

& "$Env:LOCALAPPDATA\MEGAcmd\mega-put.bat" "RetailCorrector-$version.exe" "/RetailCorrector/RetailCorrector-$version.exe"
$link = (& "$Env:LOCALAPPDATA\MEGAcmd\mega-export.bat" -a "/RetailCorrector/RetailCorrector-$version.exe").split(' ')[2]

$feats = @()
$fixes = @()
$perfs = @()
foreach ($commit in $compare.commits)
{
    $end = ($commit.title -match "^[a-z]+!?: .* \(!\d+\)$" ? "" : " ($($commit.id))" )
    if ($commit.title.StartsWith("fix")){
        $fixes += $commit.title.Replace("fix: ", "").Replace("fix!: ", "") + $end
    } elseif ($commit.title.StartsWith("feat")){
        $feats += $commit.title.Replace("feat: ", "").Replace("feat!: ", "") + $end
    } elseif ($commit.title.StartsWith("perf")){
        $perfs += $commit.title.Replace("perf: ", "").Replace("perf!: ", "") + $end
    }
}
$description = "## [$version]($CI_PROJECT_URL/compare/$prep_version...$version) ($(Get-Date -Format "yyyy-MM-dd"))`n"
if ($feats.Length -gt 0) {
    $description += "`n`n### ✨ Нововведения`n`n* "
    $description += $feats -join "`n* "
}
if ($fixes.Length -gt 0) {
    $description += "`n`n### 🐞 Исправления`n`n* "
    $description += $fixes -join "`n* "
}
if ($perfs.Length -gt 0) {
    $description += "`n`n### ⚡ Улучшение производительности`n`n* "
    $description += $perfs -join "`n* "
}

$gitlab_release_body = @{
    name = "$version"
    tag_name = "$version"
    ref = "dev"
    description = "$description"
    assets = @{
        links = @(
            @{
                name = "Дистрибутив в MEGA"
                url = "$link"
                link_type = "package"
            }
        )
    }
} | ConvertTo-Json -Depth 10 -Compress

Invoke-WebRequest "https://gitlab.com/api/v4/projects/$CI_PROJECT_ID/releases" -Headers @{"PRIVATE-TOKEN" = $CI_JOB_TOKEN} -Method Post -ContentType 'application/json' -Body $gitlab_release_body
