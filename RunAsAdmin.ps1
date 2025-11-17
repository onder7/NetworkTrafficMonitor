# Run Network Traffic Monitor as Administrator
$exePath = ".\bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe"

if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Host "Requesting Administrator privileges..." -ForegroundColor Yellow
    Start-Process -FilePath $exePath -Verb RunAs
}
else
{
    Write-Host "Already running as Administrator" -ForegroundColor Green
    & $exePath
}
