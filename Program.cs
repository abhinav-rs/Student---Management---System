using System;
using System.Linq;
using System.Threading.Tasks;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;
using StudentManagementSystem.Services;

namespace StudentManagementSystem
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IStudentRepository studentRepo = new InMemoryStudentRepository();
            ICourseRepository courseRepo = new InMemoryCourseRepository();

            StudentService studentService = new StudentService(studentRepo, courseRepo);
            CourseService courseService = new CourseService(courseRepo);

            await courseService.AddCourseAsync(new Course("CS101", "Intro to C#", 3, "Dr. Smith"));
            await courseService.AddCourseAsync(new Course("MATH200", "Calculus I", 4, "Prof. Jones"));

            bool keepRunning = true;

            while (keepRunning)
            {
                Console.WriteLine("\n=================================");
                Console.WriteLine("   Student Management System   ");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Add a New Student");
                Console.WriteLine("2. View All Students");
                Console.WriteLine("3. Check Student GPA");
                Console.WriteLine("4. Enroll Student in a Course");
                Console.WriteLine("5. Search for a Student");
                Console.WriteLine("6. Delete a Student");
                Console.WriteLine("7. Delete a Course");
                Console.WriteLine("8. Exit");
                Console.Write("\nPlease enter your choice (1-8): ");

                string choice = Console.ReadLine();
                Console.WriteLine(); 

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Student ID: ");
                        int id = int.Parse(Console.ReadLine());
                        
                        Console.Write("Enter Student Name: ");
                        string name = Console.ReadLine();
                        
                        Console.Write("Enter Student Email: ");
                        string email = Console.ReadLine();

                        if (!email.Contains("@"))
                        {
                            Console.WriteLine(" Error: Please enter a valid email address containing '@'.");
                            continue; 
                        }
                        
                        var newStudent = new Student(id, name, email, DateTime.Now);
                        await studentService.AddStudentAsync(newStudent);
                        
                        Console.WriteLine("Student added successfully!");
                        break;

                    case "2":
                        var students = await studentService.GetAllStudentsSortedAsync();
                        
                        Console.WriteLine("--- Student List ---");
                        if (students.Any())
                        {
                            foreach (var student in students)
                            {
                                Console.WriteLine($"[ID: {student.Id}] {student.Name} ({student.Email})");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No students currently in the system.");
                        }
                        break;

                    case "3":
                        Console.Write("Enter Student ID: ");
                        int gpaId = int.Parse(Console.ReadLine());
                        
                        double gpa = await studentService.CalculateGpaAsync(gpaId);
                        Console.WriteLine($"The GPA for student {gpaId} is: {gpa:F2}");
                        break;

                    case "4":
                        Console.Write("Enter Student ID: ");
                        int enrollId = int.Parse(Console.ReadLine());
                        
                        Console.Write("Enter Course Code (e.g., CS101, MATH200): ");
                        string enrollCourseCode = Console.ReadLine();

                        try 
                        {
                            await studentService.EnrollStudentAsync(enrollId, enrollCourseCode);
                            await studentService.AssignGradeAsync(enrollId, enrollCourseCode, 4.0); 
                            Console.WriteLine($" Successfully enrolled student {enrollId} in {enrollCourseCode}!");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error: {e.Message}");
                        }
                        break;

                    case "5":
                        Console.Write("Enter name or email to search: ");
                        string searchTerm = Console.ReadLine();
                        
                        var searchResults = await studentService.SearchStudentsAsync(searchTerm);
                        
                        Console.WriteLine("--- Search Results ---");
                        if (searchResults.Any())
                        {
                            foreach (var student in searchResults)
                            {
                                Console.WriteLine($"[ID: {student.Id}] {student.Name} ({student.Email})");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No students found matching that search.");
                        }
                        break;

                    case "6":
                        Console.Write("Enter Student ID to delete: ");
                        int deleteId = int.Parse(Console.ReadLine());

                        try
                        {
                            await studentService.RemoveStudentAsync(deleteId);
                            Console.WriteLine($"Successfully deleted student {deleteId} and all their enrollments.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($" Error: {e.Message}");
                        }
                        break;

                    case "7":
                        Console.Write("Enter Course Code to delete: ");
                        string courseCodeToDelete = Console.ReadLine();

                        try
                        {
                            await studentService.RemoveCourseFromAllStudentsAsync(courseCodeToDelete);
                            
                            await courseService.RemoveCourseAsync(courseCodeToDelete);
                            
                            Console.WriteLine($"Successfully deleted course {courseCodeToDelete} and cleaned up all enrollments.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error: {e.Message}");
                        }
                        break;

                    case "8":
                        keepRunning = false;
                        Console.WriteLine("Exiting the system. Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 8.");
                        break;
                }
            }
        }
    }
}