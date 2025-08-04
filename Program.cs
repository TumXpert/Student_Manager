using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text.Json;

class Student
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateTime BirthDate { get; set; } = DateTime.MinValue;
    public string Grade { get; set; }
}
class Program
{
    static List<Student> students = new List<Student>();

    const string FILE_PATH = "student.json";
    static void Main()
    {
        LoadStudentsFromFile();
        while (true)
        {
            Console.WriteLine("\n=== Student Record Manager ===");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. View All Students");
            Console.WriteLine("3. Search Student by Name");
            Console.WriteLine("4. Save Students to File");
            Console.WriteLine("5. Exit");

            Console.WriteLine("Select an option: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddStudent();
                    break;
                case "2":
                    ViewAllStudents();
                    break;
                case "3":
                    SearchStudent();
                    break;
                case "4":
                    SaveStudentsToFile();
                    break;
                case "5":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid Option");
                    break;
            }
        }
    }

    static void AddStudent()
    {
        Console.Write("Enter student First Name: ");
        string fname = Console.ReadLine();

        Console.Write("Enter the Last Name: ");
        string lname = Console.ReadLine();

        Console.Write("Enter Age: ");
        int age = int.Parse(Console.ReadLine());

        DateTime birthDate;
        while (true)
        {
            Console.Write("Enter Date of Birth (yyyy-MM-dd): ");
            string dobInput = Console.ReadLine();

            if (DateTime.TryParse(dobInput, out birthDate))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid date format. Try again.");
            }
        }

        Console.Write("Enter grade: ");
        string grade = Console.ReadLine();



        students.Add(new Student { FirstName = fname, LastName = lname, Age = age, BirthDate = birthDate, Grade = grade });

        Console.WriteLine("Student added successfully!");
        SaveStudentsToFile();
        Console.WriteLine("Student Details saved Successfully");
    }

    static void ViewAllStudents()
    {
        Console.WriteLine("\n---All Students ---");
        if (students.Count == 0)
        {
            Console.WriteLine("No students added yet.");
            return ;
        }

        foreach (var s  in students)
        {
            Console.WriteLine($"FirstName: {s.FirstName}, LastName: {s.LastName}, Age: {s.Age}, Date of Birth: {s.BirthDate:yyyy-MM-dd} Grade: {s.Grade}");

        }
    }

    static void SearchStudent()
    {
        Console.Write("Enter name to search: ");
        string searchName = Console.ReadLine().ToLower();

        var results = students.FindAll(s => s.FirstName.ToLower().Contains(searchName) || s.LastName.ToLower().Contains(searchName));

        if (results.Count == 0)
        {
            Console.WriteLine("No Matching students found.");
        }
        else {
            foreach (var s in results) 
            {
                Console.WriteLine($"Name: {s.FirstName + " " + s.LastName}, Age: {s.Age}, Grade: {s.Grade}");
            }
        }
    }

    static void LoadStudentsFromFile()
    {
        if (File.Exists(FILE_PATH)) { 
            string json = File.ReadAllText(FILE_PATH);
            if (!string.IsNullOrWhiteSpace(json))
            {
                students = JsonSerializer.Deserialize<List<Student>>(json);
                Console.WriteLine($"Loaded {students.Count} students from file.");
            }
        }
    }

    static void SaveStudentsToFile()
    {
        string json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FILE_PATH, json);
        Console.WriteLine("Students saved to file successfully.");
    }
}