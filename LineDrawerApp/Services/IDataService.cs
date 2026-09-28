// Services/IDataService.cs
using System;
using System.Threading.Tasks;

namespace WpfDrawingApp.Services
{
    public interface IDataService
    {
        Task SaveSegmentAsync(Guid sessionId, double startX, double startY, double endX, double endY, double length);
        Task<double> GetTotalLengthSqlAsync(Guid sessionId);
        Task EnsureCreatedAsync();
    }
}