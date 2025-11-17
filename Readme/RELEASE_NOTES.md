# 🎉 Release Notes - v2.0

## Network Traffic Monitor v2.0 - "Filtering Edition"

**Release Date**: 2025-01-17  
**Build**: Release  
**Platform**: Windows 11 / .NET 8.0

---

## 🆕 Yeni Özellikler

### 1. Güçlü Filtreleme Sistemi
- **Protokol Filtresi**: TCP, UDP veya tümünü göster
- **Gerçek Zamanlı Arama**: Process, IP, port, domain ile anında arama
- **Çoklu Alan Desteği**: Tek arama ile tüm alanlarda arama
- **Case-Insensitive**: Büyük/küçük harf duyarsız arama

### 2. Debug Penceresi
- Gerçek zamanlı log görüntüleme
- API çağrı sonuçları
- Hata mesajları
- Sorun giderme için ideal

### 3. Geliştirilmiş UI
- Modern filtre paneli
- Daha iyi kontrast
- Responsive tasarım
- Tooltip'ler

---

## 🔧 İyileştirmeler

### Performans
- Filtreleme algoritması optimize edildi
- Bellek kullanımı iyileştirildi
- Thread-safe operasyonlar

### Kullanılabilirlik
- Daha iyi hata mesajları
- Bilgilendirici tooltip'ler
- Kullanıcı dostu arayüz

### Dokümantasyon
- FILTERING_GUIDE.md eklendi
- TROUBLESHOOTING.md güncellendi
- README.md genişletildi

---

## 🐛 Düzeltilen Hatalar

- ✅ Veri gelmeme sorunu çözüldü
- ✅ Administrator yetki kontrolü iyileştirildi
- ✅ Exception handling geliştirildi
- ✅ UI freeze sorunu giderildi

---

## 📦 Kurulum

### Gereksinimler
- Windows 11 (veya Windows 10)
- .NET 8.0 Runtime
- Administrator yetkileri

### Hızlı Kurulum
```powershell
# 1. Projeyi derle
dotnet build --configuration Release

# 2. Çalıştır
.\RunAsAdmin.ps1
```

### Manuel Kurulum
1. `bin\Release\net8.0-windows\` klasörüne git
2. `NetworkTrafficMonitor.exe` dosyasına sağ tıkla
3. "Run as administrator" seç

---

## 🎯 Kullanım

### Temel Kullanım
1. Uygulamayı Administrator olarak başlat
2. **Start** butonuna tıkla
3. Gerçek zamanlı trafik izle

### Filtreleme
1. **Protocol** dropdown'dan TCP veya UDP seç
2. **Search** kutusuna arama terimi yaz
3. Sonuçlar anında güncellenir

### Debug
1. **Debug** butonuna tıkla
2. Log mesajlarını izle
3. Sorun varsa hata mesajlarını kontrol et

---

## 📊 Özellik Listesi

### Temel Özellikler
- ✅ Gerçek zamanlı TCP/UDP izleme
- ✅ Inbound/Outbound ayrımı
- ✅ Process bilgisi
- ✅ Port açıklamaları
- ✅ SQLite loglama

### Yeni Özellikler (v2.0)
- ✅ Protokol filtresi
- ✅ Gerçek zamanlı arama
- ✅ Debug penceresi
- ✅ Gelişmiş UI

### Gelecek Özellikler
- ⏳ Grafik gösterimi
- ⏳ Export özellikleri
- ⏳ Threat Intelligence
- ⏳ Bandwidth monitoring

---

## 🔍 Bilinen Sorunlar

### Sınırlamalar
- Domain çözümlemesi yavaşlatma nedeniyle devre dışı (v2.1'de düzeltilecek)
- Byte sayacı henüz çalışmıyor (ETW entegrasyonu gerekli)
- Paket detayları yok (gelecek sürümde)

### Workaround'lar
- Domain için IP adresi gösteriliyor
- Byte sayacı yerine bağlantı sayısı gösteriliyor

---

## 📚 Dokümantasyon

### Kılavuzlar
- [README.md](README.md) - Genel bilgi
- [QUICKSTART.md](QUICKSTART.md) - Hızlı başlangıç
- [USAGE.md](USAGE.md) - Detaylı kullanım
- [FILTERING_GUIDE.md](FILTERING_GUIDE.md) - Filtreleme rehberi
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Sorun giderme

### Teknik Dokümantasyon
- [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) - Proje özeti
- [FEATURES_v2.md](FEATURES_v2.md) - Yeni özellikler

---

## 🔐 Güvenlik

### Administrator Yetkileri
Uygulama ağ trafiğini izlemek için Administrator yetkileri gerektirir.

### Veri Gizliliği
- Tüm veriler local'de saklanır
- İnternet bağlantısı gerektirmez (DNS hariç)
- Hiçbir veri dışarı gönderilmez

### Firewall
- Windows Firewall ile uyumlu
- Antivirüs yazılımları tarafından engellenebilir

---

## 🤝 Katkıda Bulunma

### Geri Bildirim
- Bug raporları
- Özellik istekleri
- Kullanım deneyimleri

### Geliştirme
- Fork & Pull Request
- Issue açma
- Dokümantasyon iyileştirme

---

## 📝 Değişiklik Geçmişi

### v2.0 (2025-01-17)
- ➕ Protokol filtresi
- ➕ Gerçek zamanlı arama
- ➕ Debug penceresi
- 🔧 Performans iyileştirmeleri
- 📚 Dokümantasyon güncellemeleri

### v1.0 (2025-01-17)
- 🎉 İlk sürüm
- ➕ TCP/UDP izleme
- ➕ Process bilgisi
- ➕ SQLite loglama

---

## 🙏 Teşekkürler

Bu sürüm kullanıcı geri bildirimleri ile şekillendi. Özellikle:
- Filtreleme özelliği talebi
- Debug penceresi önerisi
- UI iyileştirme fikirleri

---

## 📞 Destek

### Sorun mu var?
1. [TROUBLESHOOTING.md](TROUBLESHOOTING.md) kontrol et
2. Debug penceresini aç
3. Hata mesajlarını kaydet

### Hala çözülmedi mi?
- Event Viewer'ı kontrol et
- Administrator olarak çalıştırdığından emin ol
- .NET 8.0 SDK'nın yüklü olduğunu kontrol et

---

## 📄 Lisans

MIT License - Özgürce kullanılabilir ve değiştirilebilir.

---

**Network Traffic Monitor v2.0** ile keyifli izlemeler! 🚀

**Download**: `bin\Release\net8.0-windows\NetworkTrafficMonitor.exe`
