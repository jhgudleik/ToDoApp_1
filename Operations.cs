using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp
{
    // Модель задачи
    public class Tasks
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Description { get; set; } = string.Empty;
        public string DateTime { get; set; } = string.Empty;
    }

    // Контекст базы данных
    public class Context : DbContext
    {
        public DbSet<Tasks> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=ToDoApp.db");
    }

    // Операции с задачами
    public class Operations
    {
        private readonly Context db;

        public Operations(Context context)
        {
            db = context;
        }

        public void DisplayByDueDate()
        {
            var tasks = db.Tasks.OrderBy(t => t.DateTime).ToList();
            if (!tasks.Any())
            {
                Console.WriteLine("Список дел пуст");
                return;
            }
            Console.WriteLine("Список дел по сроку исполнения:");
            for (int i = 0; i < tasks.Count; i++)
            {
                var t = tasks[i];
                Console.WriteLine($"{i + 1}. (Срок: {t.DateTime}) - {t.Category}: {t.Description} (Приоритет: {t.Priority})");
            }
        }

        public void DisplayByPriority()
        {
            var tasks = db.Tasks.OrderBy(t => t.Priority).ToList();
            if (!tasks.Any())
            {
                Console.WriteLine("Список дел пуст");
                return;
            }
            Console.WriteLine("Список дел по приоритету:");
            for (int i = 0; i < tasks.Count; i++)
            {
                var t = tasks[i];
                Console.WriteLine($"{i + 1}. (Приоритет: {t.Priority}) - {t.Category}: {t.Description} (Срок: {t.DateTime})");
            }
        }

        public void AddTask()
        {
            var task = new Tasks();

            Console.Write("Введите категорию задачи: ");
            task.Category = Console.ReadLine() ?? "";

            while (true)
            {
                Console.Write("Введите приоритет (1 - высокий, 2 - средний, 3 - низкий): ");
                if (int.TryParse(Console.ReadLine(), out int prio) && prio >= 1 && prio <= 3)
                {
                    task.Priority = prio;
                    break;
                }
                Console.WriteLine("Ошибка: введите число от 1 до 3");
            }

            Console.Write("Введите описание задачи: ");
            task.Description = Console.ReadLine() ?? "";

            Console.Write("Введите срок исполнения (гггг-мм-дд чч:мм): ");
            task.DateTime = Console.ReadLine() ?? "";

            db.Tasks.Add(task);
            Console.WriteLine("Задача добавлена! Не забудьте сохранить изменения (пункт 6).");
        }

        public void RemoveTask()
        {
            var tasks = db.Tasks.ToList();
            if (!tasks.Any())
            {
                Console.WriteLine("Нет задач для удаления.");
                return;
            }

            Console.Write("Введите номер задачи для удаления (начиная с 1): ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tasks.Count)
            {
                var taskToRemove = tasks[index - 1];
                db.Tasks.Remove(taskToRemove);
                Console.WriteLine("Задача удалена! Не забудьте сохранить изменения (пункт 6).");
            }
            else
            {
                Console.WriteLine("Ошибка: неверный номер задачи!");
            }
        }

        public void EditTask()
        {
            var tasks = db.Tasks.ToList();
            if (!tasks.Any())
            {
                Console.WriteLine("Нет задач для редактирования.");
                return;
            }

            Console.Write("Введите номер задачи для редактирования (начиная с 1): ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tasks.Count)
            {
                var task = tasks[index - 1];
                Console.WriteLine($"Редактирование задачи: {task.Category} - {task.Description}");

                Console.Write("Введите новую категорию (оставьте пустым, чтобы не менять): ");
                string newCategory = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newCategory))
                    task.Category = newCategory;

                Console.Write("Введите новый приоритет (1 - высокий, 2 - средний, 3 - низкий, оставьте пустым, чтобы не менять): ");
                string newPriority = Console.ReadLine() ?? "";
                if (int.TryParse(newPriority, out int prio) && prio >= 1 && prio <= 3)
                    task.Priority = prio;

                Console.Write("Введите новое описание (оставьте пустым, чтобы не менять): ");
                string newDescription = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newDescription))
                    task.Description = newDescription;

                Console.Write("Введите новый срок исполнения (гггг-мм-дд чч:мм, оставьте пустым, чтобы не менять): ");
                string newDateTime = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newDateTime))
                    task.DateTime = newDateTime;

                db.Tasks.Update(task);
                Console.WriteLine("Задача обновлена! Не забудьте сохранить изменения (пункт 6).");
            }
            else
            {
                Console.WriteLine("Ошибка: неверный номер задачи!");
            }
        }

        public void SaveChanges()
        {
            try
            {
                db.SaveChanges();
                Console.WriteLine("Изменения сохранены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при сохранении: " + ex.Message);
            }
        }

        public void CategorySearch()
        {
            Console.Write("Введите категорию для поиска: ");
            string category = Console.ReadLine() ?? "";

            var found = db.Tasks.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!found.Any())
            {
                Console.WriteLine("Задачи с такой категорией не найдены.");
                return;
            }

            Console.WriteLine($"Задачи в категории \"{category}\":");
            int i = 1;
            foreach (var task in found)
            {
                Console.WriteLine($"{i}. {task.Description} (Приоритет: {task.Priority}, Срок: {task.DateTime})");
                i++;
            }
        }

        public void PrioritySearch()
        {
            while (true)
            {
                Console.Write("Введите приоритет для поиска (1 - высокий, 2 - средний, 3 - низкий): ");
                if (int.TryParse(Console.ReadLine(), out int prio) && prio >= 1 && prio <= 3)
                {
                    var found = db.Tasks.Where(t => t.Priority == prio).ToList();

                    if (!found.Any())
                    {
                        Console.WriteLine("Задачи с таким приоритетом не найдены.");
                        return;
                    }

                    Console.WriteLine($"Задачи с приоритетом {prio}:");
                    int i = 1;
                    foreach (var task in found)
                    {
                        Console.WriteLine($"{i}. {task.Category}: {task.Description} (Срок: {task.DateTime})");
                        i++;
                    }
                    break;
                }
                Console.WriteLine("Ошибка: введите число от 1 до 3");
            }
        }
    }
}