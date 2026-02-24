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
                Console.WriteLine("8. Search Courses by Instructor / Credits");
                Console.WriteLine("9. Exit");
                Console.Write("\nPlease enter your choice (1-9): ");

                string choice = Console.ReadLine();
                Console.WriteLine(); 

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Student ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int id))
                        {
                            Console.WriteLine(" Error: Student ID must be a valid integer.");
                            continue;
                        }
                        
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

                        try
                        {
                            await studentService.AddStudentAsync(newStudent);
                            Console.WriteLine("Student added successfully!");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($" Error: {e.Message}");
                        }
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
                        if (!int.TryParse(Console.ReadLine(), out int gpaId))
                        {
                            Console.WriteLine(" Error: Student ID must be a valid integer.");
                            continue;
                        }
                        
                        try
                        {
                            double gpa = await studentService.CalculateGpaAsync(gpaId);
                            Console.WriteLine($"The GPA for student {gpaId} is: {gpa:F2}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($" Error: {e.Message}");
                        }
                        break;

                    case "4":
                        Console.Write("Enter Student ID: ");
                        if (!int.TryParse(Console.ReadLine(), out int enrollId))
                        {
                            Console.WriteLine(" Error: Student ID must be a valid integer.");
                            continue;
                        }
                        
                        Console.Write("Enter Course Code (e.g., CS101, MATH200): ");
                        string enrollCourseCode = Console.ReadLine();

                        Console.Write("Enter Grade (0.0 - 10.0): ");
                        if (!double.TryParse(Console.ReadLine(), out double grade) || grade < 0.0 || grade > 10.0)
                        {
                            Console.WriteLine(" Error: Grade must be a number between 0.0 and 10.0.");
                            continue;
                        }

                        try 
                        {
                            await studentService.EnrollStudentAsync(enrollId, enrollCourseCode);
                            await studentService.AssignGradeAsync(enrollId, enrollCourseCode, grade);
                            Console.WriteLine($" Successfully enrolled student {enrollId} in {enrollCourseCode} with grade {grade:F2}!");
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
                        if (!int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            Console.WriteLine(" Error: Student ID must be a valid integer.");
                            continue;
                        }

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
                        Console.Write("Enter Instructor Name to search (or press Enter to skip): ");
                        string instrName = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(instrName)) instrName = null;

                        Console.Write("Enter Credit count to search (or press Enter to skip): ");
                        string credInput = Console.ReadLine();
                        int? creditFilter = null;
                        if (!string.IsNullOrWhiteSpace(credInput))
                        {
                            if (int.TryParse(credInput, out int parsedCredits))
                                creditFilter = parsedCredits;
                            else
                            {
                                Console.WriteLine(" Error: Credit count must be a valid integer.");
                                continue;
                            }
                        }

                        var courseResults = await courseService.SearchCoursesAsync(instrName, creditFilter);

                        Console.WriteLine("--- Course Search Results ---");
                        if (courseResults.Any())
                        {
                            foreach (var course in courseResults)
                            {
                                Console.WriteLine($"[{course.Code}] {course.Name} | Credits: {course.Credits} | Instructor: {course.Instructor}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No courses found matching that search.");
                        }
                        break;

                    case "9":
                        keepRunning = false;
                        Console.WriteLine("Exiting the system. Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 9.");
                        break;
                }
            }
        }
    }
}