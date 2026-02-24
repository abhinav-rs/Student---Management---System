using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Services
{
    public class StudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly ICourseRepository _courseRepo;

        public StudentService(IStudentRepository studentRepo, ICourseRepository courseRepo)
        {
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
        }

        public async Task AddStudentAsync(Student student)
        {
            await _studentRepo.AddAsync(student);
        }

        public async Task UpdateStudentAsync(Student student)
        {
            await _studentRepo.UpdateAsync(student);
        }

        public async Task RemoveStudentAsync(int id)
        {
            await _studentRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<Student>> GetAllStudentsSortedAsync()
        {
            var allStudents = await _studentRepo.GetAllAsync();
            return allStudents.OrderBy(s => s.Name);
        }

        public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
        {
            var allStudents = await _studentRepo.GetAllAsync();
            if (string.IsNullOrWhiteSpace(searchTerm)) return allStudents;

            return allStudents.Where(s => 
                s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        public async Task EnrollStudentAsync(int studentId, string courseCode)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null) throw new Exception($"Student with ID {studentId} not found.");

            var course = await _courseRepo.GetByCodeAsync(courseCode);
            if (course == null) throw new Exception($"Course {courseCode} does not exist.");

            bool isAlreadyEnrolled = student.Enrollments.Any(e => e.CourseCode == courseCode);
            if (isAlreadyEnrolled) throw new Exception($"Student is already enrolled in {courseCode}.");

            var newEnrollment = new Enrollment(studentId, courseCode, 0.0);
            student.AddEnrollment(newEnrollment);
            await _studentRepo.UpdateAsync(student);
        }

        public async Task AssignGradeAsync(int studentId, string courseCode, double newGrade)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null) throw new Exception($"Student {studentId} not found.");

            var enrollment = student.Enrollments.FirstOrDefault(e => e.CourseCode == courseCode);
            if (enrollment == null) throw new Exception($"Not enrolled in {courseCode}.");

            enrollment.AssignGrade(newGrade);
            await _studentRepo.UpdateAsync(student);
        }

        public async Task<double> CalculateGpaAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student == null || !student.Enrollments.Any()) return 0.0; 

            double totalQualityPoints = 0;
            int totalCredits = 0;

            foreach (var enrollment in student.Enrollments)
            {
                var course = await _courseRepo.GetByCodeAsync(enrollment.CourseCode);
                if (course != null)
                {
                    totalQualityPoints += (enrollment.Grade * course.Credits);
                    totalCredits += course.Credits;
                }
            }

            if (totalCredits == 0) return 0.0;
            return totalQualityPoints / totalCredits;
        }

        public async Task<double> GetSchoolAverageGpaAsync()
        {
            var allStudents = await _studentRepo.GetAllAsync();
            var allGpas = new List<double>();

            foreach (var student in allStudents)
            {
                double gpa = await CalculateGpaAsync(student.Id);
                allGpas.Add(gpa);
            }

            if (allGpas.Any()) return allGpas.Average();
            
            return 0.0; 
        }

        public async Task RemoveCourseFromAllStudentsAsync(string courseCode)
        {
        var allStudents = await _studentRepo.GetAllAsync();
    
        foreach (var student in allStudents)
        {
        student.Enrollments.RemoveAll(s => s.CourseCode == courseCode);
        }

        }
    }
}