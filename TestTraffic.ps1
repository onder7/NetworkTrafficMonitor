# Test script to verify traffic capture works
Write-Host "Testing Network Traffic Capture..." -ForegroundColor Cyan

# Test TCP connections using netstat
Write-Host "`nTCP Connections (netstat):" -ForegroundColor Yellow
netstat -ano | Select-Object -First 20

# Test if we can get process info
Write-Host "`nProcess Info Test:" -ForegroundColor Yellow
Get-Process | Select-Object -First 5 | Format-Table Id, ProcessName, CPU

Write-Host "`nIf you see data above, the APIs should work." -ForegroundColor Green
Write-Host "Now run the application as Administrator." -ForegroundColor Green
