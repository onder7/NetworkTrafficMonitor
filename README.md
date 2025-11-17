# Network Traffic Monitor - Windows 11

Gerçek zamanlı ağ trafiği izleme uygulaması. Windows üzerinde tüm gelen/giden bağlantıları, portları, protokolleri ve process bilgilerini gösterir. Bağlantıyı seçerek Block/Allow olarak windows firewalla ekleme yapabilirsiniz

## Özellikler

✅ Gerçek zamanlı TCP/UDP bağlantı izleme
✅ Inbound/Outbound trafik ayrımı
✅ Process bazlı trafik analizi
✅ **🔪 Process sonlandırma (Close/Force Kill)** 🆕
✅ **🛡️ Sistem process koruması** 🆕
✅ **📜 Geçmiş izleme ve analiz**
✅ **🗄️ Otomatik database yönetimi (7 gün)**
✅ **📊 Top processes ve IP istatistikleri**
✅ **📈 LiveCharts bandwidth grafiği**
✅ **🔍 TCP flag analizi (SYN, ACK, FIN, RST)**
✅ **📦 Paket detay görüntüleme**
✅ **Gerçek zamanlı byte sayacı**
✅ **Global bandwidth monitoring (bytes/sec)**
✅ **Process bazlı byte istatistikleri**
✅ **Güçlü filtreleme ve arama sistemi**
✅ Protokol filtresi (TCP/UDP)
✅ Gerçek zamanlı arama (process, IP, port, domain)
✅ Port açıklamaları ve protokol tanımlama
✅ SQLite veritabanı ile loglama
✅ Modern dark theme arayüz
✅ Domain çözümlemesi (Reverse DNS)
✅ Firewall durumu kontrolü
✅ Debug penceresi

## Gereksinimler

- Windows 11
- .NET 8.0 SDK
- Administrator yetkileri (ağ trafiği izleme için gerekli)

## Kurulum

1. Projeyi derleyin:
```powershell
cd NetworkTrafficMonitor
dotnet restore
dotnet build
```

2. Uygulamayı Administrator olarak çalıştırın:

**Yöntem 1 - PowerShell Script ile:**
```powershell
.\RunAsAdmin.ps1
```

**Yöntem 2 - Manuel:**
- `bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe` dosyasına sağ tıklayın
- "Run as administrator" seçeneğini seçin

**Yöntem 3 - Komut satırı:**
```powershell
Start-Process .\bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe -Verb RunAs
```

## Kullanım

1. Uygulamayı başlatın (Administrator olarak)
2. "Start" butonuna tıklayın
3. Gerçek zamanlı trafik izlemeye başlayın
4. Farklı sekmelerde Outbound, Inbound ve Process trafiğini görüntüleyin

### Filtreleme ve Arama

- **Protocol**: TCP veya UDP seçerek filtreleme yapın
- **Search**: Process adı, IP, port, domain ile arama yapın
- Gerçek zamanlı filtreleme - yazdıkça sonuçlar güncellenir!

Detaylı bilgi için: [FILTERING_GUIDE.md](/Readme/FILTERING_GUIDE.md)

## Teknik Detaylar

- **TrafficService**: Windows API (iphlpapi.dll) kullanarak TCP/UDP bağlantılarını alır
- **PacketCaptureService**: 1 saniye aralıklarla trafik verilerini toplar
- **DatabaseService**: SQLite ile tüm bağlantıları loglar
- **ProtocolExplainService**: Port numaralarına göre açıklama sağlar
- **FirewallService**: Windows Firewall durumunu kontrol eder

## Geliştirme Notları

Bu minimal implementasyon şunları içerir:
- Temel TCP/UDP bağlantı izleme
- Process bilgisi eşleştirme
- SQLite loglama
- Material Design UI

Geliştirilmesi gerekenler:
- ETW (Event Tracing for Windows) entegrasyonu
- Paket seviyesi yakalama
- Gerçek byte sayacı
- LiveCharts grafikleri
- Export özellikleri (JSON/CSV)
- Threat Intelligence API entegrasyonu

## Lisans

MIT License
