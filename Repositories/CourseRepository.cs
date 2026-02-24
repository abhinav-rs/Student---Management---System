using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync(); 
        Task<Course> GetByCodeAsync(string code);   
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(string code);
    }

    public class InMemoryCourseRepository : ICourseRepository
    {
        private readonly List<Course> _courses = new List<Course>();

        public Task<IEnumerable<Course>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Course>>(_courses);
        }

        public Task<Course> GetByCodeAsync(string code)
        {
            var course = _courses.FirstOrDefault(c => c.Code == code);
            return Task.FromResult(course);
        }

        public Task AddAsync(Course course)
        {
            _courses.Add(course);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Course course)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string code)
        {
            var course = _courses.FirstOrDefault(c => c.Code == code);
            if (course != null)
            {
                _courses.Remove(course);
            }
            return Task.CompletedTask;
        }
    }
}