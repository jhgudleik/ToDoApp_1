using System;
using Microsoft.EntityFrameworkCore;

using ToDoApp;

using var db = new Context();
db.Database.EnsureCreated();

var ops = new Operations(db);

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

while (true)
{
    Console.WriteLine("\nСписок дел:");
    Console.WriteLine("1. Все задачи по сроку исполнения");
    Console.WriteLine("2. Все задачи по приоритету");
    Console.WriteLine("3. Добавить задачу");
    Console.WriteLine("4. Удалить задачу");
    Console.WriteLine("5. Редактировать задачу");
    Console.WriteLine("6. Сохранить изменения");
    Console.WriteLine("7. Искать по категории");
    Console.WriteLine("8. Искать по приоритету");
    Console.WriteLine("9. Выход");
    Console.Write("Выбор: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Ошибка: неверный ввод!");
        continue;
    }

    switch (choice)
    {
        case 1: ops.DisplayByDueDate(); break;
        case 2: ops.DisplayByPriority(); break;
        case 3: ops.AddTask(); break;
        case 4: ops.RemoveTask(); break;
        case 5: ops.EditTask(); break;
        case 6: ops.SaveChanges(); break;
        case 7: ops.CategorySearch(); break;
        case 8: ops.PrioritySearch(); break;
        case 9:
            Console.WriteLine("Выход");
            return;
        default:
            Console.WriteLine("Ошибка: неверный выбор!");
            break;
    }
}
