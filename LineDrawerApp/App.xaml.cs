// App.xaml.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using WpfDrawingApp.Data;
using WpfDrawingApp.Services;
using WpfDrawingApp.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace WpfDrawingApp
{
    public partial class App : Application
    {
        // Глобальный провайдер сервисов
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // 1. Регистрация DbContext (PostgreSQL)
            // Замените строку подключения на свои данные!
            string connectionString = "Host=localhost;Port=5432;Database=wpf_drawing_db;Username=postgres;Password=1111;";

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // 2. Регистрация сервисов и ViewModel
            services.AddScoped<IDataService, DataService>();
            services.AddTransient<MainViewModel>();

            // 3. Регистрация главного окна
            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            // 4. Инициализация БД (создание таблиц, если их нет)
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbService = scope.ServiceProvider.GetRequiredService<IDataService>();
                await dbService.EnsureCreatedAsync();
            }

            // 5. Запуск окна
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}