namespace Task03.Models;

public class Farm(Stream output, EventHandler? handler)
{
    private readonly Mechanic _mechanic = new(output, handler);
    private readonly Warehouse _warehouse = new(output, handler);
    private readonly Loader _loader = new(output, handler);
    private readonly Random _random = new();

    public void Start()
    {
        _warehouse.WarehouseFull += OnWarehouseFull;

        while (true)
        {
            Thread.Sleep(1000); // Имитация времени между доениями
            var milkAmount = _random.Next(10, 30);
            _warehouse.AddMilk(milkAmount);

            if (_random.Next(0, 100) >= 10) continue; // 10% вероятность поломки
            var a = "Оборудование сломалось!\n"u8.ToArray();
            var task = output.WriteAsync(a).AsTask();
            task.Wait();
            output.Flush();
            handler?.Invoke(this, EventArgs.Empty);
            _mechanic.FixEquipment();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private void OnWarehouseFull()
    {
        _loader.TakeMilk(_warehouse);
    }
}