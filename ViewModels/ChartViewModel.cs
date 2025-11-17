using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace NetworkTrafficMonitor.ViewModels
{
    public class ChartViewModel : BaseViewModel
    {
        private readonly int _maxDataPoints = 60; // 60 saniye = 1 dakika
        private readonly Queue<double> _uploadData = new();
        private readonly Queue<double> _downloadData = new();
        private readonly ObservableCollection<double> _uploadValues = new();
        private readonly ObservableCollection<double> _downloadValues = new();

        public ObservableCollection<ISeries> Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public ChartViewModel()
        {
            // İlk değerleri doldur
            for (int i = 0; i < _maxDataPoints; i++)
            {
                _uploadValues.Add(0);
                _downloadValues.Add(0);
            }

            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Name = "Upload",
                    Values = _uploadValues,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.LimeGreen) { StrokeThickness = 2 },
                    GeometrySize = 0,
                    LineSmoothness = 0.5
                },
                new LineSeries<double>
                {
                    Name = "Download",
                    Values = _downloadValues,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.DeepSkyBlue) { StrokeThickness = 2 },
                    GeometrySize = 0,
                    LineSmoothness = 0.5
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Time (seconds ago)",
                    NamePaint = new SolidColorPaint(SKColors.White),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 }
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Bytes/sec",
                    NamePaint = new SolidColorPaint(SKColors.White),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 },
                    Labeler = value => FormatBytes((long)value)
                }
            };
        }

        public void AddDataPoint(long uploadBytes, long downloadBytes)
        {
            // Queue'ya ekle
            _uploadData.Enqueue(uploadBytes);
            _downloadData.Enqueue(downloadBytes);

            // Maksimum veri noktası aşıldıysa eski verileri çıkar
            if (_uploadData.Count > _maxDataPoints)
            {
                _uploadData.Dequeue();
                _downloadData.Dequeue();
            }

            // ObservableCollection'ı güncelle
            _uploadValues.Clear();
            _downloadValues.Clear();

            foreach (var value in _uploadData)
            {
                _uploadValues.Add(value);
            }

            foreach (var value in _downloadData)
            {
                _downloadValues.Add(value);
            }
        }

        public void Clear()
        {
            _uploadData.Clear();
            _downloadData.Clear();
            _uploadValues.Clear();
            _downloadValues.Clear();

            for (int i = 0; i < _maxDataPoints; i++)
            {
                _uploadValues.Add(0);
                _downloadValues.Add(0);
            }
        }

        private string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
            return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
        }
    }
}
