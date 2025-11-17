# 📡 Network Traffic Monitor – Windows 7/10/11/S2026/2019/2022

**Real-time network traffic analyzer with firewall control, process monitoring, logs, statistics, and bandwidth charts.**

**Windows 11 için gerçek zamanlı ağ trafiği izleme, process analizi, firewall kontrolü ve istatistik özellikleri sunar.**


---
Relsease

https://drive.google.com/file/d/14VLrIvpcfdVaA6WlEJ5MIJEZhJpMEEV_/view?usp=sharing

https://ondernet.net/windows-icin-gelismis-ag-trafigi-izleme-araci-network-traffic-monitor

https://www.youtube.com/watch?v=n0Cp9PXhuyA

# 🌍 Contents / İçerikler

* [English](#english)
* [Türkçe](#türkçe)

---

# English

## 📡 Network Traffic Monitor – Windows 11

A powerful real-time network traffic monitoring application for Windows. Shows all inbound/outbound connections, ports, protocols, and process information. You can select any connection and instantly Block/Allow it using Windows Firewall.

---

## ✨ Features

* ✅ Real-time TCP/UDP connection monitoring
* ✅ Inbound/Outbound traffic separation
* ✅ Per-process traffic analytics
* ✅ **🔪 Process termination (Close / Force Kill)**
* ✅ **🛡️ System process protection**
* ✅ **📜 Log history & analytics**
* ✅ **🗄️ Automatic database cleanup (7 days)**
* ✅ **📊 Top processes & IP statistics**
* ✅ **📈 LiveCharts real-time bandwidth graph**
* ✅ **🔍 TCP flag analysis (SYN, ACK, FIN, RST)**
* ✅ **📦 Packet details viewer**
* ✅ Real-time byte counter
* ✅ Global bandwidth monitoring (bytes/sec)
* ✅ Per-process byte statistics
* ✅ Advanced filtering & searching
* ✅ Protocol filter (TCP/UDP)
* ✅ Real-time search (process, IP, port, domain)
* ✅ Port descriptions & protocol detection
* ✅ SQLite database logging
* ✅ Modern dark theme UI
* ✅ Reverse DNS resolution
* ✅ Firewall status monitoring
* ✅ Debug console

---

## 📦 Requirements

* Windows 11
* .NET 8.0 SDK
* Administrator privileges (required for low-level network monitoring)

---

## 🚀 Installation

### 1. Build the project

```powershell
cd NetworkTrafficMonitor
dotnet restore
dotnet build
```

### 2. Run with administrator privileges

**Method 1 – PowerShell script**

```powershell
.\RunAsAdmin.ps1
```

**Method 2 – Manual**

* Right-click `bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe`
* Select **Run as administrator**

**Method 3 – Command**

```powershell
Start-Process .\bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe -Verb RunAs
```

---

## 🧭 Usage

1. Launch the application (as Administrator)
2. Click **Start**
3. View real-time network traffic
4. Navigate between Inbound, Outbound, and Processes tabs

### 🔍 Filtering & Search

* Filter by protocol (TCP/UDP)
* Search by process name, IP, port, domain
* Results update instantly as you type

---

## ⚙️ Technical Details

* **TrafficService** — Uses Windows API (`iphlpapi.dll`) to read active connections
* **PacketCaptureService** — Collects bandwidth and packet data every second
* **DatabaseService** — Saves all connections to SQLite
* **ProtocolExplainService** — Provides explanations for port numbers
* **FirewallService** — Reads/updates Windows Firewall rules

---

## 🛠️ Development Notes

Current implementation includes:

* Basic TCP/UDP monitoring
* Process information matching
* SQLite logging
* Material Design UI

Planned improvements:

* ETW (Event Tracing for Windows) integration
* Packet-level capture
* Real byte counter
* LiveCharts improvements
* Export (JSON/CSV)
* Threat Intelligence API integration

---

## 📄 License

This project is licensed under the **MIT License**.

---

# Türkçe

## 📡 Network Traffic Monitor – Windows 11

Windows için geliştirilmiş, gerçek zamanlı ağ trafiği izleme uygulaması. Tüm gelen/giden bağlantıları, portları, protokolleri ve process bilgilerini gösterir. İsterseniz bağlantıya sağ tıklayıp **Block/Allow** olarak Windows Firewall'a kural ekleyebilirsiniz.

---

## ✨ Özellikler

* ✅ Gerçek zamanlı TCP/UDP bağlantı izleme
* ✅ Inbound/Outbound trafik ayrımı
* ✅ Process bazlı trafik analizi
* ✅ **🔪 Process sonlandırma (Close / Force Kill)**
* ✅ **🛡️ Sistem process koruması**
* ✅ **📜 Geçmiş kayıtları ve analiz**
* ✅ **🗄️ Otomatik database temizliği (7 gün)**
* ✅ **📊 En çok trafik üreten process ve IP istatistikleri**
* ✅ **📈 LiveCharts anlık bandwidth grafiği**
* ✅ **🔍 TCP flag analizi (SYN, ACK, FIN, RST)**
* ✅ **📦 Paket detay görüntüleme**
* ✅ Gerçek zamanlı byte sayacı
* ✅ Global bandwidth izleme
* ✅ Process bazlı byte istatistikleri
* ✅ Gelişmiş filtreleme ve arama
* ✅ Protokol filtresi (TCP/UDP)
* ✅ Gerçek zamanlı arama (process, IP, port, domain)
* ✅ Port açıklamaları ve protokol tanımlama
* ✅ SQLite log sistemi
* ✅ Modern dark theme UI
* ✅ Reverse DNS çözümlemesi
* ✅ Firewall durumu kontrolü
* ✅ Debug konsolu

---

## 📦 Gereksinimler

* Windows 11
* .NET 8.0 SDK
* Administrator yetkileri

---

## 🚀 Kurulum

### 1. Projeyi derleyin

```powershell
cd NetworkTrafficMonitor
dotnet restore
dotnet build
```

### 2. Yönetici olarak çalıştırın

**Yöntem 1 – PowerShell Script**

```powershell
.\RunAsAdmin.ps1
```

**Yöntem 2 – Manuel**

* `bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe`
  → **Sağ tıklayın → Run as Administrator**

**Yöntem 3 – Komut**

```powershell
Start-Process .\bin\Debug\net8.0-windows\NetworkTrafficMonitor.exe -Verb RunAs
```

---

## 🧭 Kullanım

1. Uygulamayı yönetici olarak çalıştırın
2. **Start** butonuna basın
3. Anlık ağ trafiğini izlemeye başlayın
4. Inbound, Outbound ve Process sekmelerini kullanın

### 🔍 Filtreleme ve Arama

* Protokol filtresi (TCP/UDP)
* Process, IP, port, domain ile arama
* Yazdıkça anında sonuç güncellenir

---

## ⚙️ Teknik Detaylar

* **TrafficService** — Windows API (`iphlpapi.dll`) ile bağlantıları okur
* **PacketCaptureService** — Her 1 saniyede trafik verisi toplar
* **DatabaseService** — Tüm bağlantıları SQLite’a kaydeder
* **ProtocolExplainService** — Port/protokol açıklamaları sağlar
* **FirewallService** — Windows Firewall ile iletişim kurar

---

## 🛠️ Geliştirme Notları

Mevcut özellikler:

* Temel TCP/UDP izleme
* Process eşleştirme
* SQLite loglama
* Modern UI

Planlananlar:

* ETW entegrasyonu
* Paket seviyesi yakalama
* Gerçek byte sayacı
* LiveCharts geliştirmeleri
* Export (CSV/JSON)
* Threat Intelligence entegrasyonu

---

## 📄 Lisans

Bu proje **MIT License** ile lisanslanmıştır.

---

Hazır!
İstersen bunu **GitHub için otomatik dosya yapısı (README_TR.md + README_EN.md + LICENSE)** formatına çevirebilirim.
