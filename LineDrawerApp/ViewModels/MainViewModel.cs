// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;
using WpfDrawingApp.Services;

namespace WpfDrawingApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private double _totalLength;

        [ObservableProperty]
        private string _statusMessage = "Начните рисование...";

        public Guid CurrentSessionId { get; private set; }

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            CurrentSessionId = Guid.NewGuid();
        }

        // Метод вызывается из View при движении мыши
        public async Task RecordSegmentAsync(double startX, double startY, double endX, double endY, double length)
        {
            await _dataService.SaveSegmentAsync(CurrentSessionId, startX, startY, endX, endY, length);
            StatusMessage = $"Отрезок сохранен. Длина: {length:F2}";
        }

        [RelayCommand]
        private async Task GetTotalLengthAsync()
        {
            try
            {
                TotalLength = await _dataService.GetTotalLengthSqlAsync(CurrentSessionId);
                StatusMessage = $"Общая длина линии (из БД): {TotalLength:F2} px";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка SQL: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ClearCanvas()
        {
            // Сбрасываем сессию, чтобы новое рисование не смешивалось со старым в БД
            CurrentSessionId = Guid.NewGuid();
            TotalLength = 0;
            StatusMessage = "Холст очищен. Начните новое рисование.";

            // Событие для View, чтобы оно очистило Polyline
            RequestClearCanvas?.Invoke(this, EventArgs.Empty);
        }

        // Событие для очистки UI (так как ViewModel не должна знать о UI контролах)
        public event EventHandler RequestClearCanvas;
    }
}