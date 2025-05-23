param(
    [string]$certFolder = ".\certs",
    [string]$aspNetProjectPath = ".\..\students-attendances-server\Attendances.Systems\Attendances.System.WebApi\Certificates",
    [string]$viteProjectPath = ".\..\students-attendances-client\certificates"
)

# Проверка папок
foreach ($path in @($aspNetProjectPath, $viteProjectPath)) {
    if (-not (Test-Path $path)) {
        New-Item -ItemType Directory -Path $path -Force | Out-Null
        Write-Host "✅ Создана папка: $path"
    }
}

# Копируем ASP.NET Core сертификаты
Copy-Item "$certFolder\aspnetcore.pfx" -Destination $aspNetProjectPath -Force
Copy-Item "$certFolder\aspnetcore.crt" -Destination $aspNetProjectPath -Force

# Копируем Vite сертификаты
Copy-Item "$certFolder\vite.key" -Destination $viteProjectPath -Force
Copy-Item "$certFolder\vite.crt" -Destination $viteProjectPath -Force

Write-Host "✅ Сертификаты успешно экспортированы:"
Write-Host "👉 ASP.NET Core → $aspNetProjectPath"
Write-Host "👉 Vite          → $viteProjectPath"
