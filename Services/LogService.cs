using biometricService.Data;
using biometricService.Data.Entities;
using biometricService.Interfaces;

namespace biometricService.Services
{
    public class LogService : ILogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }
        public void Log(string message)
        {
            var entity = new LogTransaction
            {               
                TransactionDetails = message,
                TimeStamp = DateTime.UtcNow,
            };

            _context.LogTransaction.Add(entity);
            _context.SaveChanges();
        }
    }
}
