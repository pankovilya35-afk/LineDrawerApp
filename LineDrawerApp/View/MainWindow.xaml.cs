using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using WpfDrawingApp.ViewModels;

namespace WpfDrawingApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private bool _isDrawing;
        private Point _previousPoint;

        public MainWindow()
        {
            InitializeComponent();

            // Получаем ViewModel из DI контейнера
            _viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
            this.DataContext = _viewModel;

            // Подписываемся на событие очистки UI
            _viewModel.RequestClearCanvas += (s, e) =>
            {
                CurrentLine.Points.Clear();
            };
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
            _previousPoint = e.GetPosition(DrawingCanvas);

            // Захватываем мышь, чтобы отслеживать движение даже за пределами Canvas
            DrawingCanvas.CaptureMouse();
            CurrentLine.Points.Add(_previousPoint);
        }

        private async void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;

            Point currentPoint = e.GetPosition(DrawingCanvas);

            // Добавляем точку для визуального отображения
            CurrentLine.Points.Add(currentPoint);

            // Вычисляем длину отрезка
            double length = CalculateDistance(_previousPoint, currentPoint);

            // Сохраняем в БД через ViewModel (передаем чистые данные, без MouseEventArgs)
            await _viewModel.RecordSegmentAsync(
                _previousPoint.X, _previousPoint.Y,
                currentPoint.X, currentPoint.Y,
                length);

            _previousPoint = currentPoint;
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
            DrawingCanvas.ReleaseMouseCapture();
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}