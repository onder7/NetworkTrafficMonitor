# Sorun Giderme Kılavuzu

## Veri Gelmiyor / Boş Ekran

### 1. Administrator Yetkisi Kontrolü

**En yaygın sorun budur!** Uygulama mutlaka Administrator olarak çalıştırılmalıdır.

#### Kontrol:
```powershell
# PowerShell'i Administrator olarak açın ve çalıştırın:
.\RunAsAdmin.ps1
```

veya

```powershell
Start-Process .\bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe -Verb RunAs
```

#### Manuel Kontrol:
1. `NetworkTrafficMonitor.exe` dosyasına sağ tıklayın
2. "Run as administrator" seçeneğini seçin
3. UAC (User Account Control) uyarısında "Yes" deyin

### 2. Debug Penceresi ile Kontrol

1. Uygulamayı başlatın
2. **"Debug"** butonuna tıklayın
3. **"Start"** butonuna tıklayın
4. Debug penceresinde şu mesajları görmelisiniz:
   ```
   [HH:mm:ss] Monitoring started!
   [HH:mm:ss] GetTcpConnections: First call result=...
   [HH:mm:ss] Got X TCP connections
   [HH:mm:ss] Got Y UDP connections
   [HH:mm:ss] Total connections: Z
   ```

### 3. Hata Mesajları

#### "Access Denied" / "Error code 5"
- **Çözüm**: Administrator olarak çalıştırın
- Windows API'leri ağ bilgilerine erişmek için yüksek yetki gerektirir

#### "Buffer size is 0"
- **Çözüm**: Bilgisayarınızda aktif bağlantı olmayabilir
- Bir web tarayıcısı açın veya internet kullanın
- Tekrar "Start" butonuna tıklayın

#### "No connections found"
- **Çözüm**: 
  1. İnternet bağlantınızı kontrol edin
  2. Firewall'un uygulamayı engellemediğini kontrol edin
  3. Antivirus yazılımını geçici olarak devre dışı bırakın

### 4. Manuel Test

PowerShell'de test edin:

```powershell
# TCP bağlantılarını göster
netstat -ano | Select-Object -First 20

# Process listesi
Get-Process | Select-Object -First 10
```

Eğer yukarıdaki komutlar çalışıyorsa, uygulama da çalışmalıdır.

### 5. Yeniden Derleme

Bazen temiz bir build gerekebilir:

```powershell
dotnet clean
dotnet build
.\RunAsAdmin.ps1
```

### 6. .NET SDK Kontrolü

```powershell
dotnet --version
```

.NET 8.0 veya üzeri olmalıdır. Değilse:
- https://dotnet.microsoft.com/download adresinden indirin

### 7. Windows Firewall Kontrolü

```powershell
# Firewall durumunu kontrol et
netsh advfirewall show currentprofile

# Uygulamayı firewall'a ekle (opsiyonel)
netsh advfirewall firewall add rule name="NetworkTrafficMonitor" dir=in action=allow program="FULL_PATH_TO_EXE" enable=yes
```

### 8. Antivirus / Security Software

Bazı güvenlik yazılımları ağ izleme uygulamalarını engelleyebilir:
- Geçici olarak devre dışı bırakın
- Veya uygulamayı whitelist'e ekleyin

### 9. Logları Kontrol Edin

Debug penceresi açıkken şunları kontrol edin:

#### Başarılı Çalışma:
```
[12:34:56] GetTcpConnections: First call result=122, bufferSize=4096
[12:34:56] GetTcpConnections: Second call result=0
[12:34:56] Got 45 TCP connections
[12:34:56] Got 23 UDP connections
[12:34:56] Total connections: 68
```

#### Hatalı Çalışma:
```
[12:34:56] GetTcpConnections: First call result=5, bufferSize=0
[12:34:56] GetTcpConnections: Buffer size is 0, no connections?
[12:34:56] Got 0 TCP connections
```

Error code 5 = Access Denied → Administrator gerekli!

### 10. Sistem Gereksinimleri

- ✅ Windows 11 (veya Windows 10)
- ✅ .NET 8.0 SDK
- ✅ Administrator yetkileri
- ✅ Aktif internet bağlantısı

### 11. Bilinen Sınırlamalar

- **Sanal Makineler**: VM'lerde ağ izleme sınırlı olabilir
- **VPN**: VPN kullanıyorsanız bazı bağlantılar görünmeyebilir
- **Proxy**: Proxy arkasında bazı veriler eksik olabilir

### 12. Hala Çalışmıyor mu?

1. **Event Viewer'ı kontrol edin**:
   ```
   Windows Logs → Application
   ```
   NetworkTrafficMonitor ile ilgili hataları arayın

2. **Dependency Walker ile kontrol edin**:
   - iphlpapi.dll yüklü mü?
   - Eksik DLL var mı?

3. **Process Monitor (Sysinternals)**:
   - Uygulamanın hangi API çağrılarını yaptığını görün
   - Hangi çağrıların başarısız olduğunu kontrol edin

### 13. Test Senaryosu

Adım adım test:

```powershell
# 1. Administrator PowerShell açın
Start-Process powershell -Verb RunAs

# 2. Proje dizinine gidin
cd D:\csharp\firewall\NetworkTrafficMonitor

# 3. Derleyin
dotnet build

# 4. Çalıştırın
.\bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe

# 5. Uygulamada:
#    - Debug butonuna tıklayın
#    - Start butonuna tıklayın
#    - Debug penceresini izleyin

# 6. Başka bir pencerede trafik oluşturun:
Start-Process chrome "https://google.com"

# 7. 2-3 saniye bekleyin, veriler gelmeye başlamalı
```

### 14. Alternatif Test

Eğer hala çalışmıyorsa, basit bir test:

```powershell
# Test scripti çalıştır
.\TestTraffic.ps1
```

Bu script netstat çıktısını gösterir. Eğer bu çalışıyorsa, sorun uygulama kodundadır.

## İletişim

Sorun devam ediyorsa:
1. Debug penceresinin screenshot'ını alın
2. Event Viewer loglarını kontrol edin
3. Hata mesajlarını kaydedin

---

**En Yaygın Çözüm**: Administrator olarak çalıştırın! 🔑
