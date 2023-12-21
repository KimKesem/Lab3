using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using System.Data.SQLite;
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
                    Console.WriteLine("Загрузить данные  из Json");
                    break;
                case "6":
                    SaveDataToXml();
                    Console.WriteLine("Сохранить в Xml");
                    break;
                case "7":
                    LoadDataFromXml();
                    Console.WriteLine("Загрузить данные  из Xml");
                    break;
                case "8" :
                    SaveDataToSQLite();
                    break;
                case "9" :
                    SaveDataToSQLite();
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
        Console.WriteLine("4. Сохранить файл в Json");
        Console.WriteLine("5.  Загрузить данные из существующего списка в формате Json");
        Console.WriteLine("6.  Сохранить файл в Xml");
        Console.WriteLine("7. Загрузить данные из существующего списка в формате Xml");
        
        Console.Write("Выберите пункт меню: ");
    }



        // Existing code...

        static void SaveDataToJson()
        {
            string jsonData = JsonSerializer.Serialize(tasks);
            File.WriteAllText("D:\\hueta\\Stasks.json", jsonData);
            Console.WriteLine("Данные сохранены в формате JSON.");
        }
       
        static void LoadDataFromJson()
        {
            if (File.Exists("tasks.json"))
            {
                string jsonData = File.ReadAllText("D:\\hueta\\Ltasks.json");
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
            using (FileStream stream = new FileStream("D:\\hueta\\Stasks.xml", FileMode.Create))
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
                using (FileStream stream = new FileStream("D:\\hueta\\Ltasks.xml", FileMode.Open))
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
        static void SaveDataToSQLite()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=tasks.db;Version=3;"))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Tasks (Title TEXT, Description TEXT, DueDate TEXT, Tags TEXT)";
                    command.ExecuteNonQuery();

                    foreach (Task task in tasks)
                    {
                        command.CommandText = $"INSERT INTO Tasks (Title, Description, DueDate, Tags) VALUES ('{task.Title}', '{task.Description}', '{task.DueDate.ToString("yyyy-MM-dd")}', '{string.Join(",", task.Tags)}')";
                        command.ExecuteNonQuery();
                    }
                }
            }

            Console.WriteLine("Данные сохранены в базе данных SQLite.");
        }

        static void LoadDataFromSQLite()
        {
            tasks.Clear();

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=tasks.db;Version=3;"))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT * FROM Tasks";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader["Title"].ToString();
                            string description = reader["Description"].ToString();
                            DateTime dueDate = DateTime.ParseExact(reader["DueDate"].ToString(), "yyyy-MM-dd", null);
                            List<string> tags = reader["Tags"].ToString().Split(',').ToList();

                            Task task = new Task(title, description, dueDate, tags);
                            tasks.Add(task);
                        }
                    }
                }
            }

            Console.WriteLine("Данные загружены из базы данных SQLite.");
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
