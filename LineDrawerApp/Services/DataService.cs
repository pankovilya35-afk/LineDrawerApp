// Services/DataService.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WpfDrawingApp.Data;
using WpfDrawingApp.Models;

namespace WpfDrawingApp.Services
{
    public class DataService : IDataService
    {
        private readonly AppDbContext _context;

        public DataService(AppDbContext context)
        {
            _context = context;
        }

        public async Task EnsureCreatedAsync()
        {
            // В реальном проекте используйте миграции. Здесь для простоты - создание БД
            await _context.Database.EnsureCreatedAsync();
        }

        public async Task SaveSegmentAsync(Guid sessionId, double startX, double startY, double endX, double endY, double length)
        {
            var segment = new LineSegment
            {
                SessionId = sessionId,
                StartX = startX,
                StartY = startY,
                EndX = endX,
                EndY = endY,
                Length = length,
                CreatedAt = DateTime.UtcNow
            };

            _context.LineSegments.Add(segment);
            await _context.SaveChangesAsync();
        }

        // ТРЕБОВАНИЕ: Получить общую длину средствами SQL
        public async Task<double> GetTotalLengthSqlAsync(Guid sessionId)
        {
            // Используем сырой SQL запрос к PostgreSQL
            string sql = @"
                SELECT COALESCE(SUM(length), 0) 
                FROM line_segments 
                WHERE ""SessionId"" = {0}";

            // В EF Core 8+ можно использовать SqlQueryRaw для скалярных типов
            var result = await _context.Database
                .SqlQueryRaw<decimal>(sql, sessionId)
                .SingleAsync();

            return (double)result;
        }
    }
}