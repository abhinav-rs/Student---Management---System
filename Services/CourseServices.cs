using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Services
{
    public class CourseService
    {
        private readonly ICourseRepository _courseRepo;

        public CourseService(ICourseRepository courseRepo)
        {
            _courseRepo = courseRepo;
        }

        public async Task AddCourseAsync(Course course)
        {
            await _courseRepo.AddAsync(course);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            await _courseRepo.UpdateAsync(course);
        }

        public async Task RemoveCourseAsync(string code)
        {
            await _courseRepo.DeleteAsync(code);
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _courseRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Course>> SearchCoursesAsync(string instructorName, int? credits)
        {
            var allCourses = await _courseRepo.GetAllAsync();

            var filteredCourses = allCourses.Where(c => 
                (instructorName != null && c.Instructor.Contains(instructorName,StringComparison.OrdinalIgnoreCase)) ||  c.Credits == credits);

            return filteredCourses;
        }
    }
}