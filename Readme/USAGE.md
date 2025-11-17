# Network Traffic Monitor - Kullanım Kılavuzu

## Başlangıç

### 1. Uygulamayı Başlatma

Uygulama **Administrator yetkileri** ile çalıştırılmalıdır:

```powershell
.\RunAsAdmin.ps1
```

veya

```powershell
Start-Process .\bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe -Verb RunAs
```

### 2. İzlemeyi Başlatma

1. Uygulama açıldığında üst kısımda **"Start"** butonuna tıklayın
2. Durum "Monitoring..." olarak değişecektir
3. Trafik verileri 1 saniye aralıklarla güncellenecektir

### 3. Sekmeleri Kullanma

#### 📤 Outbound Traffic (Giden Trafik)
- Bilgisayarınızdan internete giden tüm bağlantıları gösterir
- Process adı, PID, protokol, IP adresleri, portlar
- Domain çözümlemesi (örn: google.com)
- Bağlantı durumu (Established, Time_Wait, vb.)

#### 📥 Inbound Traffic (Gelen Trafik)
- Dışarıdan bilgisayarınıza gelen bağlantıları gösterir
- Listening portları gösterir
- Hangi uygulamaların port dinlediğini gösterir

#### 📊 Process Traffic (Uygulama Bazlı)
- Her uygulamanın kaç bağlantısı olduğunu gösterir
- En çok bağlantı yapan uygulamaları listeler
- Process ID ve isim bilgisi

## Özellikler

### Gerçek Zamanlı İzleme
- 1 saniye aralıklarla güncelleme
- Otomatik domain çözümlemesi
- Process bilgisi eşleştirme

### Port Açıklamaları
Yaygın portlar için otomatik açıklama:
- 443: HTTPS - Secure Web
- 80: HTTP - Web Traffic
- 53: DNS - Domain Name System
- 3389: RDP - Remote Desktop
- 445: SMB - File Sharing
- ve daha fazlası...

### Veritabanı Loglama
- Tüm bağlantılar SQLite veritabanına kaydedilir
- `traffic.db` dosyasında saklanır
- Geçmiş trafik analizi için kullanılabilir

## Butonlar

- **Start**: İzlemeyi başlatır
- **Stop**: İzlemeyi durdurur
- **Clear**: Ekrandaki verileri temizler (veritabanı etkilenmez)

## Sorun Giderme

### "Access Denied" Hatası
- Uygulamayı Administrator olarak çalıştırdığınızdan emin olun
- Windows Firewall'un uygulamayı engellemediğini kontrol edin

### Hiç Veri Görünmüyor
- Administrator yetkileri ile çalıştığınızdan emin olun
- "Start" butonuna tıkladığınızdan emin olun
- Bilgisayarınızda aktif internet bağlantısı olduğunu kontrol edin

### Uygulama Yavaş Çalışıyor
- Çok fazla bağlantı varsa güncelleme yavaşlayabilir
- "Clear" butonuna tıklayarak ekranı temizleyin
- Gereksiz uygulamaları kapatın

## Teknik Detaylar

### Kullanılan API'ler
- `GetExtendedTcpTable` - TCP bağlantıları
- `GetExtendedUdpTable` - UDP bağlantıları
- `Process.GetProcessById` - Process bilgileri
- `Dns.GetHostEntryAsync` - Domain çözümlemesi

### Veritabanı Şeması
```sql
CREATE TABLE Connections (
    Id INTEGER PRIMARY KEY,
    LocalAddress TEXT,
    LocalPort INTEGER,
    RemoteAddress TEXT,
    RemotePort INTEGER,
    Protocol TEXT,
    ProcessName TEXT,
    ProcessId INTEGER,
    Direction TEXT,
    State TEXT,
    Domain TEXT,
    Description TEXT,
    IsBlocked INTEGER,
    Timestamp TEXT
)
```

## Gelecek Özellikler

- [ ] Paket seviyesi yakalama (ETW)
- [ ] Gerçek byte sayacı
- [ ] Grafik gösterimi (LiveCharts)
- [ ] Export (JSON/CSV/PCAP)
- [ ] Filtreleme ve arama
- [ ] Threat Intelligence entegrasyonu
- [ ] Firewall kural yönetimi

## Lisans

MIT License - Özgürce kullanabilir ve değiştirebilirsiniz.
