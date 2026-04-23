# 企業整批匯款管理系統 - 核心邏輯測試 (.NET 8 & Vue 3)

## 專案背景與架構規範
這是一個簡化版的銀行匯款管理系統。為了符合企業級開發規範，本專案要求**嚴格的職責分離（Separation of Concerns）**：
- **Controller**：只負責接收 HTTP Request、驗證輸入、呼叫 Service，以及回傳正確的 HTTP Status Code (200, 400, 404)。
- **Service**：負責處理核心商業邏輯與資料庫狀態檢核。

## 核心任務與商業邏輯
當匯款資料尚未送往主機前，經辦人員可執行取消。
- `Status` 定義：`0: 待覆核`, `1: 交易中`, `2: 已完成`, `9: 已取消`。
- **絕對規則**：**只有狀態為 `0` 的資料才可以被取消（更新為 `9`）**。
- **資安要求**：必須防範前端連點造成的併發（Concurrency）問題，避免重複扣款或狀態異常。

## 您的實作範圍 (TODOs)
1. **依賴注入配置 (`Program.cs`)**
   - 請將 Service 註冊到 DI 容器中。
2. **服務層 (`Services/RemittanceService.cs`)**
   - 實作 `IRemittanceService` 介面，完成 `CancelRemittance` 的商業邏輯與防呆。
3. **控制層 (`Controllers/RemittanceController.cs`)**
   - 透過建構子注入 Service。
   - 呼叫 Service 並根據結果回傳相對應的 `IActionResult`。
4. **前端串接 (`wwwroot/index.html`)**
   - 完成 Vue 內的 `cancelItem(id)` 函數，並根據 API 回傳結果更新 UI。
5. **架構問答 (請直接回答於下方)**
   - 雖然本測試使用 In-Memory List 模擬資料庫，但在正式的 SQL Server 環境中，您會如何撰寫 T-SQL 或 Entity Framework Core 程式碼，來確保「多個使用者同時對同一筆資料按下取消」時，不會發生 Race Condition？
   - **您的回答：** 
     為了防止 Race Condition（例如 Double-click 或併發 Request），在正式 SQL Server 環境中，我會採用以下幾種方式之一：

     **1. T-SQL (狀態條件更新 - 樂觀鎖概念)：**
     利用 SQL 語句在 `UPDATE` 時同時檢查當前狀態
     ```sql
     UPDATE Remittances 
     SET Status = 9 
     WHERE Id = @Id AND Status = 0;
     ```
     然後檢查受影響的資料列數 (`@@ROWCOUNT`)。如果回傳 0，代表該筆資料不存在或狀態已經不是 `0` (可能已被其他人取消或進入處理)，此時可回傳錯誤訊息給前端。

     **2. Entity Framework Core (樂觀併發控制 Optimistic Concurrency)：**
     - **直接對狀態加 `[ConcurrencyCheck]`：** 對 `Status` 欄位標記 `[ConcurrencyCheck]`，EF Core 產生的 SQL 會自動加上 `WHERE Id = @Id AND Status = 0`。利用 `DbUpdateConcurrencyException` 來處理併發。

## 提交要求
1. 請使用 GitHub Public Repository 提交。
2. **請務必多次 Commit**（例如：API完成、前端完成、Bug修復），讓我們了解您的開發思路。