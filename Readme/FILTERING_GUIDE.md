# 🔍 Filtreleme ve Arama Kılavuzu

## Yeni Özellikler

Artık Network Traffic Monitor'da güçlü filtreleme ve arama özellikleri var!

## 📊 Filtreleme Seçenekleri

### 1. Protokol Filtresi

Her sekmenin üstünde **Protocol** dropdown menüsü var:

- **All**: Tüm bağlantıları göster (TCP + UDP)
- **TCP**: Sadece TCP bağlantılarını göster
- **UDP**: Sadece UDP bağlantılarını göster

**Kullanım:**
```
Protocol: [All ▼]  →  [TCP ▼]  →  Sadece TCP bağlantıları görünür
```

### 2. Arama (Search)

Gerçek zamanlı arama özelliği - yazdıkça filtreler!

**Aranabilir Alanlar:**

#### Outbound & Inbound Sekmeleri:
- ✅ Process adı (örn: "chrome", "firefox")
- ✅ Local IP adresi
- ✅ Remote IP adresi
- ✅ Local Port
- ✅ Remote Port
- ✅ Domain adı
- ✅ Açıklama (Description)
- ✅ Bağlantı durumu (State)

#### Process Traffic Sekmesi:
- ✅ Process adı
- ✅ Process ID (PID)

## 💡 Kullanım Örnekleri

### Örnek 1: Chrome Bağlantılarını Bul
```
Search: chrome
```
→ Sadece Chrome'un bağlantıları görünür

### Örnek 2: HTTPS Trafiğini Göster
```
Protocol: TCP
Search: 443
```
→ Sadece 443 portundaki (HTTPS) TCP bağlantıları

### Örnek 3: Belirli Bir IP'yi İzle
```
Search: 142.250
```
→ Google IP'leri (142.250.x.x) görünür

### Örnek 4: DNS Sorgularını Göster
```
Protocol: UDP
Search: 53
```
→ DNS sorguları (port 53)

### Örnek 5: Established Bağlantılar
```
Search: established
```
→ Sadece aktif bağlantılar

### Örnek 6: Belirli Process ID
```
Search: 1234
```
→ PID 1234'ün bağlantıları

## 🎯 Filtreleme Mantığı

### AND Mantığı
Protokol ve arama **birlikte** çalışır:

```
Protocol: TCP  +  Search: chrome
= Sadece Chrome'un TCP bağlantıları
```

### OR Mantığı (Arama İçinde)
Arama tüm alanlarda arar:

```
Search: chrome
= Process adı "chrome" VEYA IP'de "chrome" VEYA açıklamada "chrome"
```

## 🔄 Gerçek Zamanlı Güncelleme

- Filtreler **gerçek zamanlı** çalışır
- Yeni bağlantılar otomatik olarak filtrelenir
- Arama kutusuna yazdıkça sonuçlar güncellenir
- Protokol değiştirince anında filtreler

## 📋 DataGrid Özellikleri

### Sıralama
Herhangi bir sütun başlığına tıklayarak sıralayabilirsiniz:
- İlk tık: Artan sıralama (A→Z, 0→9)
- İkinci tık: Azalan sıralama (Z→A, 9→0)
- Üçüncü tık: Sıralama kaldır

### Sütun Genişliği
Sütun başlıkları arasındaki çizgileri sürükleyerek genişlik ayarlayabilirsiniz.

### Seçim
Bir satıra tıklayarak seçebilirsiniz (gelecekte detay görüntüleme için kullanılacak).

## 🎨 Görsel Rehber

```
┌─────────────────────────────────────────────────────────────┐
│ 📤 Outbound Traffic                                          │
├─────────────────────────────────────────────────────────────┤
│ Protocol: [TCP ▼]     Search: [chrome____________]          │
├─────────────────────────────────────────────────────────────┤
│ Process  │ PID  │ Protocol │ Local IP    │ Remote IP       │
│──────────┼──────┼──────────┼─────────────┼─────────────────│
│ chrome   │ 1234 │ TCP      │ 192.168.1.5 │ 142.250.x.x     │
│ chrome   │ 1234 │ TCP      │ 192.168.1.5 │ 172.217.x.x     │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Performans İpuçları

### Hızlı Filtreleme
- Protokol filtresi çok hızlıdır
- Arama biraz daha yavaş olabilir (çok fazla bağlantı varsa)

### Çok Fazla Veri Varsa
1. Protokol filtresini kullanın (TCP veya UDP)
2. Spesifik arama yapın (process adı veya port)
3. "Clear" butonuna tıklayarak eski verileri temizleyin

## 💾 Filtreleme ve Veritabanı

**Önemli:** Filtreler sadece **görüntülemeyi** etkiler!

- Tüm bağlantılar veritabanına kaydedilir
- Filtreler veritabanını etkilemez
- Filtreyi kaldırınca tüm veriler tekrar görünür

## 🔮 Gelecek Özellikler

- [ ] Kayıtlı filtre profilleri
- [ ] Regex desteği
- [ ] Port aralığı filtresi (örn: 8000-9000)
- [ ] IP aralığı filtresi
- [ ] Zaman bazlı filtreleme
- [ ] Favorilere ekleme
- [ ] Export filtered data

## 📝 Kısayollar

| Aksiyon | Kısayol |
|---------|---------|
| Arama kutusuna odaklan | Ctrl+F (gelecekte) |
| Filtreyi temizle | ESC (gelecekte) |
| Tümünü seç | Ctrl+A (gelecekte) |

## 🎓 İpuçları

1. **Boş arama**: Tüm verileri gösterir
2. **Büyük/küçük harf**: Önemli değil (case-insensitive)
3. **Kısmi eşleşme**: "chro" yazarsanız "chrome" bulur
4. **Sayılar**: Port veya PID araması için sayı yazın
5. **IP adresleri**: Kısmi IP de aranabilir (örn: "192.168")

## 🐛 Sorun Giderme

### Arama Çalışmıyor
- Arama kutusuna yazdığınızdan emin olun
- Protokol filtresini "All" yapın
- "Clear" ile verileri temizleyip yeniden başlatın

### Hiçbir Sonuç Yok
- Arama kriterlerinizi genişletin
- Protokol filtresini kontrol edin
- Monitoring'in çalıştığından emin olun

### Yavaş Çalışıyor
- Çok fazla bağlantı varsa normal
- Daha spesifik filtre kullanın
- "Clear" ile eski verileri temizleyin

---

**Keyifli filtreleme!** 🎯
