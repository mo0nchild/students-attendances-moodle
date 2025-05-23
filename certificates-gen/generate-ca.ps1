#Параметры
param(
    [string]$caName = "student_attendance",
    [string]$certFolder = ".\certs"
)

# Путь к OpenSSL
$openssl = "openssl"

# Создание папки для сертификатов
if (-not (Test-Path $certFolder)) {
    New-Item -ItemType Directory -Path $certFolder | Out-Null
}

# Генерация собственного CA
& $openssl genrsa -out "$certFolder\myCA.key" 2048
& $openssl req -x509 -new -nodes -key "$certFolder\myCA.key" -sha256 -days 1825 -out "$certFolder\myCA.pem" -subj "/CN=$caName"

Write-Host "✅ CA успешно создан в $certFolder"
