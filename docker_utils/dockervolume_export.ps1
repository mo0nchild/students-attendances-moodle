param (
    [string]$VolumeName,
    [string]$BackupPath = ".\backup.tar.gz"
)

Write-Host "Создание архива из volume: $VolumeName"

docker run --rm `
    -v "${VolumeName}:/data" `
    -v "${PWD}:/backup" `
    alpine tar czf "/backup/$(Split-Path -Leaf $BackupPath)" -C /data .

Write-Host "Архив создан по пути: $BackupPath"
