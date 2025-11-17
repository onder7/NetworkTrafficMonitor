# 📜 History Tracking & Database Management

## Yeni Özellik: Geçmiş İzleme

Network Traffic Monitor artık tüm bağlantı geçmişini kaydediyor ve analiz edebiliyorsunuz!

---

## 🎯 Özellikler

### 1. Otomatik Veri Saklama

**Varsayılan Ayarlar:**
- Veriler **7 gün** saklanır
- Daha eski kayıtlar otomatik silinir
- Uygulama başlangıcında temizlik yapılır

### 2. History Sekmesi

Yeni **History** sekmesinde:
- Geçmiş bağlantıları görüntüleme
- Tarih aralığı seçimi
- Arama ve filtreleme
- Database istatistikleri

### 3. Database İstatistikleri

Gösterilen bilgiler:
- **Total Records**: Toplam kayıt sayısı
- **Database Size**: Veritabanı boyutu (MB/GB)
- **Oldest Record**: En eski kayıt tarihi
- **Data Span**: Veri aralığı

---

## 📊 History Sekmesi Kullanımı

### Hızlı Zaman Aralıkları

Tek tıkla:
- **Last 1h**: Son 1 saat
- **Last 24h**: Son 24 saat
- **Last 7d**: Son 7 gün

### Özel Tarih Aralığı

1. **From** tarihini seç
2. **To** tarihini seç
3. **Load History** butonuna tıkla

### Arama

Search kutusuna yazın:
- Process adı
- IP adresi
- Domain
- Açıklama

### Örnek Kullanım

```
1. History sekmesine git
2. "Last 24h" butonuna tıkla
3. Search: "chrome"
4. Load History
5. Chrome'un son 24 saatteki tüm bağlantılarını gör
```

---

## 🗄️ Database Yönetimi

### Otomatik Temizleme

**Ne zaman?**
- Uygulama her başlatıldığında
- 7 günden eski kayıtlar silinir

**Neden?**
- Veritabanı boyutunu kontrol altında tutar
- Performansı korur
- Disk alanı tasarrufu

### Manuel Temizleme

**Clean Old (7d+)** butonu:
1. History sekmesine git
2. "Clean Old (7d+)" butonuna tıkla
3. Onay ver
4. 7 günden eski kayıtlar silinir

### Database İstatistikleri

**Refresh Stats** butonu:
- Güncel istatistikleri gösterir
- Toplam kayıt sayısı
- Veritabanı boyutu
- En eski/yeni kayıt

---

## 📈 Analiz Özellikleri

### Top Processes

En çok bağlantı yapan 10 process:
```
chrome: 1,234 connections
firefox: 567 connections
discord: 234 connections
...
```

### Top Remote IPs

En çok bağlantı yapılan 10 IP:
```
142.250.x.x: 456 connections (Google)
104.16.x.x: 234 connections (Cloudflare)
...
```

---

## 🎨 Görünüm

### History Sekmesi

```
┌─────────────────────────────────────────────────────────────┐
│ Database Statistics                                          │
│ Total: 12,345  Size: 5.67 MB  Oldest: 2025-01-10 12:00     │
│                                    [Refresh] [Clean Old 7d+] │
├─────────────────────────────────────────────────────────────┤
│ Quick: [Last 1h] [Last 24h] [Last 7d]                       │
│ From: [2025-01-17] To: [2025-01-17] Search: [chrome]        │
│                                              [Load History]  │
├─────────────────────────────────────────────────────────────┤
│ Time       │ Process │ Protocol │ Local IP    │ Remote IP   │
├────────────┼─────────┼──────────┼─────────────┼─────────────┤
│ 12:34:56   │ chrome  │ TCP      │ 192.168.1.5 │ 142.250.x.x │
│ 12:34:55   │ chrome  │ TCP      │ 192.168.1.5 │ 172.217.x.x │
│ ...                                                           │
├─────────────────────────────────────────────────────────────┤
│ Showing 234 records                                          │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔧 Teknik Detaylar

### Database Schema

```sql
CREATE TABLE Connections (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
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

### Yeni Metodlar

```csharp
// Geçmiş kayıtları getir
public List<ConnectionInfo> GetConnectionHistory(
    DateTime? startDate = null, 
    DateTime? endDate = null, 
    int limit = 1000)

// Eski kayıtları temizle
public void CleanOldRecords(int daysToKeep = 7)

// Database istatistikleri
public DatabaseStats GetDatabaseStats()

// En çok bağlantı yapan process'ler
public Dictionary<string, int> GetTopProcesses(
    int topN = 10, 
    DateTime? startDate = null, 
    DateTime? endDate = null)

// En çok bağlantı yapılan IP'ler
public Dictionary<string, int> GetTopRemoteIPs(
    int topN = 10, 
    DateTime? startDate = null, 
    DateTime? endDate = null)
```

### DatabaseStats Sınıfı

```csharp
public class DatabaseStats
{
    public int TotalRecords { get; set; }
    public DateTime? OldestRecord { get; set; }
    public DateTime? NewestRecord { get; set; }
    public long DatabaseSizeBytes { get; set; }
    public string DatabaseSizeFormatted { get; }
    public TimeSpan? DataSpan { get; }
}
```

---

## 💡 Kullanım Senaryoları

### Senaryo 1: Geçmiş Analizi

```
Soru: Chrome dün hangi sitelere bağlandı?

1. History sekmesine git
2. "Last 24h" seç
3. Search: "chrome"
4. Load History
5. Tüm bağlantıları gör
```

### Senaryo 2: Şüpheli Aktivite

```
Soru: Gece 3'te hangi uygulamalar aktifti?

1. History sekmesine git
2. From: Dün 03:00
3. To: Dün 04:00
4. Load History
5. Gece aktif olan process'leri gör
```

### Senaryo 3: Bandwidth Analizi

```
Soru: Hangi uygulama en çok bağlantı yapıyor?

1. History sekmesine git
2. "Last 7d" seç
3. Load History
4. Top Processes listesine bak
```

### Senaryo 4: IP Takibi

```
Soru: Belirli bir IP'ye ne zaman bağlandım?

1. History sekmesine git
2. Search: "142.250.x.x"
3. Load History
4. O IP'ye yapılan tüm bağlantıları gör
```

---

## ⚙️ Ayarlar

### Veri Saklama Süresi

Varsayılan: **7 gün**

Değiştirmek için:
```csharp
// MainViewModel.cs içinde
_databaseService.CleanOldRecords(30); // 30 gün
```

### Maksimum Kayıt Sayısı

History yüklerken limit:
```csharp
// Varsayılan: 5000 kayıt
GetConnectionHistory(startDate, endDate, 10000); // 10000 kayıt
```

---

## 📊 Performans

### Database Boyutu

Ortalama kayıt boyutu: ~200 bytes

| Süre | Kayıt Sayısı | Boyut |
|------|--------------|-------|
| 1 gün | ~10,000 | ~2 MB |
| 7 gün | ~70,000 | ~14 MB |
| 30 gün | ~300,000 | ~60 MB |

### Sorgu Performansı

- **Load History**: < 1 saniye (5000 kayıt)
- **Clean Old Records**: < 2 saniye
- **Get Stats**: < 100ms

---

## 🐛 Sorun Giderme

### Database Çok Büyüdü

**Çözüm:**
1. History sekmesine git
2. "Clean Old (7d+)" butonuna tıkla
3. Veya manuel olarak `traffic.db` dosyasını sil

### Yavaş Yükleme

**Çözüm:**
1. Daha kısa tarih aralığı seç
2. Arama filtresi kullan
3. Limit değerini azalt

### Eski Kayıtlar Silinmiyor

**Çözüm:**
1. Uygulamayı yeniden başlat (otomatik temizlik)
2. Manuel "Clean Old (7d+)" kullan

---

## 🔮 Gelecek Geliştirmeler

### v5.0 Planları
- [ ] Export history (CSV/JSON)
- [ ] Grafik gösterimi (timeline)
- [ ] Otomatik raporlama
- [ ] Email bildirimleri

### v6.0 Planları
- [ ] Cloud backup
- [ ] Multi-device sync
- [ ] Advanced analytics
- [ ] Machine learning insights

---

## 🎓 İpuçları

### Disk Alanı Tasarrufu

```
1. Düzenli temizlik yapın
2. Sadece önemli verileri saklayın
3. Export yapıp eski verileri arşivleyin
```

### Hızlı Analiz

```
1. Quick time range butonlarını kullanın
2. Spesifik arama yapın
3. Top Processes/IPs listelerine bakın
```

### Veri Güvenliği

```
1. traffic.db dosyasını yedekleyin
2. Hassas verileri export edin
3. Düzenli backup alın
```

---

**Artık tüm network geçmişinizi takip edebilirsiniz!** 📜🔍

**Database Location**: `traffic.db` (uygulama klasöründe)
