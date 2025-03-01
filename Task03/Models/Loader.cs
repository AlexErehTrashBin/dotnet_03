namespace Task03.Models;

public class Loader(Stream output, EventHandler? handler)
{
    // ReSharper disable once MemberCanBeMadeStatic.Global
#pragma warning disable CA1822
    public void TakeMilk(Warehouse warehouse)
#pragma warning restore CA1822
    {
        var task = output.WriteAsync("Погрузчик забирает молоко со склада...\n"u8.ToArray()).AsTask();
        task.Wait();   
        output.Flush();
        handler?.Invoke(this, EventArgs.Empty);
        warehouse.ClearWarehouse();
    }
}