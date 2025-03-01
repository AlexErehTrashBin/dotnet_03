namespace Task03.Models;

public class Mechanic(Stream output, EventHandler? handler)
{
    public void FixEquipment()
    {
        var task1 = output.WriteAsync("Механик чинит оборудование...\n"u8.ToArray()).AsTask();
        task1.Wait();
        output.Flush();
        handler?.Invoke(null, EventArgs.Empty);
        Thread.Sleep(2000); // Имитация времени починки
        var task = output.WriteAsync("Оборудование починено!\n"u8.ToArray()).AsTask();
        task.Wait();
        output.Flush();
        handler?.Invoke(null, EventArgs.Empty);
    }
}