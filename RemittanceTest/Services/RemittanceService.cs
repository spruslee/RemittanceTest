using RemittanceTest.Models;

namespace RemittanceTest.Services
{
    public class RemittanceService : IRemittanceService
    {
        // 模擬資料庫 (靜態變數確保跨 Request 資料一致)
        private static readonly List<Remittance> _db = new()
        {
            new Remittance { Id = 1, AccountName = "測試企業A", Amount = 50000, Status = 0 },
            new Remittance { Id = 2, AccountName = "測試企業B", Amount = 12000, Status = 1 }, // 不可取消
            new Remittance { Id = 3, AccountName = "測試企業C", Amount = 30000, Status = 0 }
        };

        // 提示：如何確保多執行緒下的資料安全？
        private static readonly object _lockObj = new object();

        public (bool IsSuccess, string Message) CancelRemittance(int id)
        {
            // TODO: 請在此處實作「取消」的商業邏輯與防併發檢核
            lock (_lockObj)
            {
                var remittance = _db.Find(r => r.Id == id);
                if (remittance == null)
                {
                    return (false, "查無此資料");
                }
                if (remittance.Status != 0)
                {
                    return (false, "資料狀態不正確，無法取消");
                }
                remittance.Status = 9;
                return (true, "取消成功");
            }
        }
    }
}