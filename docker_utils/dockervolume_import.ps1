param (
    [string]$VolumeName,
    [string]$BackupPath = ".\backup.tar.gz"
)

Write-Host "Создание volume: $VolumeName"
docker volume create $VolumeName | Out-Null

Write-Host "Распаковка архива в volume: $VolumeName"

docker run --rm `
    -v "${VolumeName}:/data" `
    -v "${PWD}:/backup" `
    alpine sh -c "cd /data && tar xzf /backup/$(Split-Path -Leaf $BackupPath)"

Write-Host "Импорт завершён"
