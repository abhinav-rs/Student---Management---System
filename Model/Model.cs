using System;
using System.Collections.Generic;

namespace StudentManagementSystem.Models
{
    public class Student
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTime EnrollmentDate { get; private set; }
        
        public List<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();
        public Student(int id, string name, string email, DateTime enrollmentDate)
        {
            Id = id;
            Name = name;
            Email = email;
            EnrollmentDate = enrollmentDate;
        }
         public void UpdateEmail(string newEmail)
        {
        Email = newEmail;
        }
        public void AddEnrollment(Enrollment newEnrollment)
        {
        Enrollments.Add(newEnrollment);
        }
    }
    public class Course
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public int Credits { get; private set; }
        public string Instructor { get; private set; }

        public Course(string code, string name, int credits, string instructor)
        {
            Code = code;
            Name = name;
            Credits = credits;
            Instructor = instructor;
        }
        public void UpdateCredits(int newCredits)
        {
            if (newCredits > 0)
            {
                Credits = newCredits;
            }
        }
        public void UpdateInstructor(string newInstructor)
        {
            Instructor = newInstructor;
        }   
    }

    public class Enrollment
    {
        public int StudentId { get; private set; }
        public string CourseCode { get; private set; }
        public double Grade { get; private set; }

        public Enrollment(int studentId, string courseCode, double grade)
        {
            StudentId = studentId;
            CourseCode = courseCode;
            Grade = grade;
        }
        public void AssignGrade(double newGrade)
        {
            if (newGrade >= 0.0 && newGrade <= 10.0)
            {
                Grade = newGrade;
            }
            else 
            {
                throw new ArgumentOutOfRangeException("Grade must be between 0.0 and 10.0");
            }
        }

    }
}