using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    
    
    static List<Task> tasks = new List<Task>();

    static void Main()
    {
        bool running = true;
        while (running)
        {
            DisplayMainMenu();
            string userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    SearchTasks();
                    break;
                case "3":
                    running = false;
                    Console.WriteLine("Программа завершена.");
                    break;
                case "4":
                    SaveDataToJson();
                    Console.WriteLine("Сохранить в Json");
                    break;
                case "5":
                    LoadDataFromJson();
                    break;
                case "6":
                    SaveDataToXml();
                    
                    break;
                case "7":
                    LoadDataFromXml();
                    
                    break;
                
                
                default:
                    Console.WriteLine("Некорректный ввод. Попробуйте ещё раз.");
                    break;
            }

            Console.WriteLine();
        }
    }

    static void DisplayMainMenu()
    {
        Console.WriteLine("Главное меню:");
        Console.WriteLine("1. Добавить новую задачу");
        Console.WriteLine("2. Поиск задач по тэгам и вывод N наиболее актуальных задач");
        Console.WriteLine("3. Выйти");
        Console.Write("Выберите пункт меню: ");
    }



        // Existing code...

        static void SaveDataToJson()
        {
            string jsonData = JsonSerializer.Serialize(tasks);
            File.WriteAllText("D:\\hueta\\tasks.json", jsonData);
            Console.WriteLine("Данные сохранены в формате JSON.");
        }
       
        static void LoadDataFromJson()
        {
            if (File.Exists("tasks.json"))
            {
                string jsonData = File.ReadAllText("tasks.json");
                tasks = JsonSerializer.Deserialize<List<Task>>(jsonData);
                Console.WriteLine("Данные загружены из формата JSON.");
            }
            else
            {
                Console.WriteLine("Файл с данными JSON не найден.");
            }
        }

        static void SaveDataToXml()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Task>));
            using (FileStream stream = new FileStream("tasks.xml", FileMode.Create))
            {
                xmlSerializer.Serialize(stream, tasks);
            }
            Console.WriteLine("Данные сохранены в формате XML.");
        }

        static void LoadDataFromXml()
        {
            if (File.Exists("tasks.xml"))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Task>));
                using (FileStream stream = new FileStream("tasks.xml", FileMode.Open))
                {
                    tasks = (List<Task>)xmlSerializer.Deserialize(stream);
                }
                Console.WriteLine("Данные загружены из формата XML.");
            }
            else
            {
                Console.WriteLine("Файл с данными XML не найден.");
            }
        }

        // Existing code...
    
    static void AddTask()
    {
        Console.WriteLine("Добавление новой задачи");

        Console.Write("Тема задачи: ");
        string title = Console.ReadLine();

        Console.Write("Описание задачи: ");
        string description = Console.ReadLine();

        Console.Write("Дата выполнения (дд-мм-гггг): ");
        string dueDateStr = Console.ReadLine();
        DateTime dueDate = DateTime.ParseExact(dueDateStr, "dd-MM-yyyy", null);

        List<string> tags = new List<string>();
        Console.WriteLine("Введите теги (пустая строка для завершения):");
        string tag = Console.ReadLine();
        while (!string.IsNullOrEmpty(tag))
        {
            tags.Add(tag);
            tag = Console.ReadLine();
        }

        Task newTask = new Task(title, description, dueDate, tags);
        tasks.Add(newTask);

        Console.WriteLine("Задача успешно добавлена.");
    }

    static void SearchTasks()
    {
        Console.WriteLine("Поиск задач");

        Console.Write("Введите ключевые слова для поиска: ");
        string[] keywords = Console.ReadLine().Split(' ');

        var matchedTasks = tasks.Where(task => keywords.All(keyword => task.Tags.Contains(keyword)))
                                .OrderBy(task => task.DueDate)
                                .Take(5);

        Console.WriteLine("Найденные задачи:");
        foreach (var task in matchedTasks)
        {
            Console.WriteLine($"Тема задачи: {task.Title}");
            Console.WriteLine($"Описание задачи: {task.Description}");
            Console.WriteLine($"Дата выполнения: {task.DueDate.ToString("dd-MM-yyyy")}");
            Console.WriteLine();
        }
    }
}

class Task
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public List<string> Tags { get; set; }

    public Task(string title, string description, DateTime dueDate, List<string> tags)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Tags = tags;
    }
}
