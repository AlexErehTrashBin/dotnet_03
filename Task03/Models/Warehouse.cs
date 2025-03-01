using System.Text;
using System.Text.Unicode;

namespace Task03.Models;

public class Warehouse(Stream output, EventHandler? handler)
{
    public event Action? WarehouseFull;

    
    private int _milkCount;
    private const int MaxCapacity = 100;
    
    public void AddMilk(int amount)
    {
        _milkCount += amount;
        var str = $"На склад добавлено {amount} литров молока. Всего: {_milkCount}\n";
        var a = Encoding.UTF8.GetBytes(str);
        var bruh = output.WriteAsync(a).AsTask();
        bruh.Wait();
        output.Flush();
        handler?.Invoke(this, EventArgs.Empty);
        if (_milkCount >= MaxCapacity)
        {
            WarehouseFull?.Invoke();
        }
    }

    public void ClearWarehouse()
    {
        _milkCount = 0;
        var task = output.WriteAsync("Склад очищен.\n"u8.ToArray()).AsTask();
        task.Wait();
        output.Flush();
        handler?.Invoke(this, EventArgs.Empty);
    }
}