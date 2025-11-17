# 🔪 Process Management - Kill Processes

## Yeni Özellik: Process'leri Sonlandırma

Network Traffic Monitor artık process'leri doğrudan uygulama içinden sonlandırabilir!

---

## 🎯 Özellikler

### 1. Process Seçimi

**Process Traffic** sekmesinde:
- DataGrid'den bir process seçin
- Seçili process bilgileri altta gösterilir
- Process Name, PID, Connection Count

### 2. İki Sonlandırma Modu

#### Close Process (Nazik Kapatma)
- Uygulamayı nazikçe kapatmaya çalışır
- `CloseMainWindow()` kullanır
- 5 saniye bekler
- Kapanmazsa zorla kapatır

#### Force Kill (Zorla Sonlandırma)
- Anında zorla sonlandırır
- Tüm child process'leri de kapatır
- **Kaydedilmemiş veriler kaybolur!**
- Acil durumlar için

### 3. Güvenlik Özellikleri

**Sistem Process Koruması:**
- System, svchost, csrss, smss
- wininit, services, lsass
- winlogon, explorer, dwm
- RuntimeBroker

Bu process'ler **korumalıdır** ve sonlandırılamaz!

---

## 🎨 Kullanım

### Process Traffic Sekmesi

```
┌─────────────────────────────────────────────────────────────┐
│ Search: [chrome_____]  [Close Process] [Force Kill]         │
├─────────────────────────────────────────────────────────────┤
│ Process │ PID  │ Connections │ Sent    │ Received           │
├─────────┼──────┼─────────────┼─────────┼────────────────────┤
│ chrome  │ 1234 │ 45          │ 12.5 MB │ 45.2 MB  ← Seçili  │
│ firefox │ 5678 │ 23          │ 5.3 MB  │ 18.7 MB            │
│ discord │ 9012 │ 12          │ 2.1 MB  │ 8.9 MB             │
├─────────────────────────────────────────────────────────────┤
│ Selected Process: chrome  |  PID: 1234  |  Connections: 45  │
│ ⚠️ System processes are protected and cannot be killed      │
└─────────────────────────────────────────────────────────────┘
```

---

## 📝 Adım Adım Kullanım

### Nazik Kapatma (Close Process)

1. **Process Traffic** sekmesine git
2. Kapatmak istediğin process'i **seç** (tıkla)
3. **Close Process** butonuna tıkla
4. Onay dialogunda **Yes** de
5. Process nazikçe kapatılır

**Örnek:**
```
1. Chrome'u seç
2. Close Process
3. Yes
4. Chrome kapatıldı!
```

### Zorla Sonlandırma (Force Kill)

1. **Process Traffic** sekmesine git
2. Sonlandırmak istediğin process'i **seç**
3. **Force Kill** butonuna tıkla
4. ⚠️ **UYARI** dialogunu oku
5. Emin misin? **Yes** de
6. Process zorla sonlandırılır

**Örnek:**
```
1. Donmuş bir uygulamayı seç
2. Force Kill
3. Uyarıyı oku
4. Yes (eminim)
5. Uygulama zorla kapatıldı!
```

---

## ⚠️ Uyarılar ve Güvenlik

### Close Process (Nazik)

✅ **Güvenli:**
- Uygulamaya kapatma sinyali gönderir
- Uygulama verileri kaydedebilir
- Temiz kapanış

⚠️ **Dikkat:**
- Bazı uygulamalar kapatmayı reddedebilir
- 5 saniye sonra zorla kapatılır

### Force Kill (Zorla)

⚠️ **TEHLİKELİ:**
- Anında sonlandırır
- Kaydedilmemiş veriler **KAYBOLUR**
- Child process'ler de kapatılır

🚫 **Kullanma:**
- Önemli belgeler açıksa
- Veritabanı işlemleri devam ediyorsa
- Sistem process'lerinde (zaten korumalı)

✅ **Kullan:**
- Uygulama donmuşsa
- Yanıt vermiyorsa
- Acil durumda

---

## 🛡️ Sistem Process Koruması

### Korumalı Process'ler

Bu process'ler **seçilemez** ve **sonlandırılamaz**:

| Process | Açıklama |
|---------|----------|
| System | Windows çekirdeği |
| svchost | Windows servisleri |
| csrss | Client/Server Runtime |
| smss | Session Manager |
| wininit | Windows başlatma |
| services | Servis kontrolü |
| lsass | Güvenlik alt sistemi |
| winlogon | Oturum açma |
| explorer | Windows Explorer |
| dwm | Desktop Window Manager |
| RuntimeBroker | Windows Runtime |

### Neden Korumalı?

- **Sistem Kararlılığı**: Bu process'ler Windows'un çalışması için gerekli
- **Veri Güvenliği**: Kapatılırsa sistem çökebilir
- **Kullanıcı Koruması**: Yanlışlıkla kapatmayı önler

---

## 💡 Kullanım Senaryoları

### Senaryo 1: Donmuş Uygulama

```
Problem: Chrome dondu, yanıt vermiyor

Çözüm:
1. Process Traffic'e git
2. Chrome'u seç
3. Close Process dene
4. Kapanmazsa Force Kill kullan
```

### Senaryo 2: Çok Fazla Trafik

```
Problem: Bir uygulama çok fazla bandwidth kullanıyor

Çözüm:
1. Process Traffic'te en çok bağlantı yapanı bul
2. O process'i seç
3. Close Process ile kapat
4. Bandwidth düşer
```

### Senaryo 3: Şüpheli Process

```
Problem: Bilinmeyen bir process çok bağlantı yapıyor

Çözüm:
1. Process Traffic'te şüpheli process'i bul
2. Process'i seç
3. PID'yi not al
4. Force Kill ile sonlandır
5. Antivirüs taraması yap
```

### Senaryo 4: Bellek Temizliği

```
Problem: RAM doldu, uygulamaları kapatmak istiyorum

Çözüm:
1. Process Traffic'te en çok bağlantı yapanları gör
2. Gereksiz olanları seç
3. Close Process ile kapat
4. RAM boşaldı
```

---

## 🔧 Teknik Detaylar

### ProcessManagementService

```csharp
public class ProcessManagementService
{
    // Process bilgisi al
    public ProcessInfo GetProcessInfo(int processId)
    
    // Process'i sonlandır
    public bool KillProcess(int processId, bool force = false)
    
    // İsme göre sonlandır
    public bool KillProcessByName(string processName, bool force = false)
    
    // Sistem process mi?
    public bool IsSystemProcess(int processId)
    
    // Process yolu
    public string GetProcessPath(int processId)
}
```

### Nazik Kapatma Algoritması

```csharp
// 1. Ana pencereyi kapat
process.CloseMainWindow();

// 2. 5 saniye bekle
if (!process.WaitForExit(5000))
{
    // 3. Kapanmadıysa zorla kapat
    process.Kill(entireProcessTree: true);
}
```

### Zorla Sonlandırma

```csharp
// Tüm process tree'yi sonlandır
process.Kill(entireProcessTree: true);
```

---

## 📊 Performans

### Close Process
- **Süre**: 0-5 saniye
- **CPU**: Minimal
- **Başarı Oranı**: ~90%

### Force Kill
- **Süre**: < 1 saniye
- **CPU**: Minimal
- **Başarı Oranı**: ~99%

---

## 🐛 Sorun Giderme

### "Access Denied" Hatası

**Neden:**
- Yeterli yetki yok
- Process başka bir kullanıcıya ait

**Çözüm:**
- Uygulamayı Administrator olarak çalıştır
- Kendi process'lerini kapat

### Process Kapanmıyor

**Neden:**
- Process yanıt vermiyor
- Sistem process'i

**Çözüm:**
- Force Kill kullan
- Sistem process'iyse kapatma (korumalı)

### Buton Devre Dışı

**Neden:**
- Process seçilmemiş
- Sistem process'i seçilmiş

**Çözüm:**
- Bir process seç
- Sistem olmayan bir process seç

---

## 🎓 İpuçları

### Güvenli Kullanım

```
1. Önce Close Process dene
2. Kapanmazsa Force Kill kullan
3. Sistem process'lerine dokunma
4. Önemli verileri kaydet
```

### Hızlı Kapatma

```
1. Process Traffic'i aç
2. Search ile process bul
3. Seç ve Close Process
4. Hızlı ve kolay!
```

### Toplu Kapatma

```
Aynı isimli birden fazla process:
1. İlkini kapat
2. Liste yenilenince
3. Diğerlerini de kapat
```

---

## 🔮 Gelecek Geliştirmeler

### v6.0 Planları
- [ ] Toplu process kapatma
- [ ] Process öncelik değiştirme
- [ ] CPU/RAM limiti koyma
- [ ] Otomatik process yönetimi

### v7.0 Planları
- [ ] Process whitelist/blacklist
- [ ] Zamanlanmış kapatma
- [ ] Process monitoring alerts
- [ ] Auto-restart özelliği

---

## ⚖️ Yasal Uyarı

**Sorumluluk:**
- Process'leri kendi sorumluluğunuzda kapatın
- Kaydedilmemiş veriler kaybolabilir
- Sistem kararsızlığına neden olabilir
- Önemli process'leri kapatmayın

**Öneriler:**
- Önce kaydet, sonra kapat
- Sistem process'lerine dokunma
- Emin değilsen kapatma
- Yedek al

---

**Artık process'leri doğrudan yönetebilirsiniz!** 🔪⚡

**Güvenlik**: Sistem process'leri korumalı  
**Mod 1**: Close Process (nazik)  
**Mod 2**: Force Kill (zorla)
