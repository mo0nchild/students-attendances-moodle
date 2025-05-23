# Путь к OpenSSL
$openssl = "openssl"

# Параметры
$caName = "student_attendance"
$localIp = "192.168.0.100"
$certPassword = "1234567890"
$certFolder = ".\certs"

# Создание папки для сертификатов
if (-not (Test-Path $certFolder)) {
    New-Item -ItemType Directory -Path $certFolder | Out-Null
}

# 1. Генерация собственного CA
& $openssl genrsa -out "$certFolder\myCA.key" 2048
& $openssl req -x509 -new -nodes -key "$certFolder\myCA.key" -sha256 -days 1825 -out "$certFolder\myCA.pem" -subj "/CN=$caName"

# 2. Генерация сертификата для ASP.NET Core
& $openssl genrsa -out "$certFolder\aspnetcore.key" 2048
& $openssl req -new -key "$certFolder\aspnetcore.key" -out "$certFolder\aspnetcore.csr" -subj "/CN=$localIp"

# 3. Создание конфига для ASP.NET Core
$aspnetExt = @"
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
subjectAltName = @alt_names

[alt_names]
IP.1 = $localIp
DNS.1 = localhost
"@
$aspnetExt | Set-Content "$certFolder\aspnetcore.ext"

# 4. Подпись сертификата ASP.NET Core
& $openssl x509 -req -in "$certFolder\aspnetcore.csr" -CA "$certFolder\myCA.pem" -CAkey "$certFolder\myCA.key" -CAcreateserial -out "$certFolder\aspnetcore.crt" -days 825 -sha256 -extfile "$certFolder\aspnetcore.ext"

# 5. Конвертация в PFX
& $openssl pkcs12 -export -out "$certFolder\aspnetcore.pfx" -inkey "$certFolder\aspnetcore.key" -in "$certFolder\aspnetcore.crt" -certfile "$certFolder\myCA.pem" -passout pass:$certPassword

# 6. Генерация сертификата для Vite
& $openssl genrsa -out "$certFolder\vite.key" 2048
& $openssl req -new -key "$certFolder\vite.key" -out "$certFolder\vite.csr" -subj "/CN=$localIp"

# 7. Создание конфига для Vite
$viteExt = @"
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
subjectAltName = @alt_names

[alt_names]
IP.1 = $localIp
DNS.1 = localhost
"@
$viteExt | Set-Content "$certFolder\vite.ext"

# 8. Подпись сертификата Vite
& $openssl x509 -req -in "$certFolder\vite.csr" -CA "$certFolder\myCA.pem" -CAkey "$certFolder\myCA.key" -CAcreateserial -out "$certFolder\vite.crt" -days 825 -sha256 -extfile "$certFolder\vite.ext"

Write-Host "✅ Сертификаты успешно созданы в папке $certFolder"
Write-Host "👉 Сертификат CA $certFolder\myCA.pem в доверенные корневые сертификаты на своих устройствах."
