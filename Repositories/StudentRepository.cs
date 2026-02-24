using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student> GetByIdAsync(int id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
    }

    public class InMemoryStudentRepository : IStudentRepository
    {
        private readonly List<Student> _students = new List<Student>();

        public Task<IEnumerable<Student>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Student>>(_students);
        }

        public Task<Student> GetByIdAsync(int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(student);
        }

        public Task AddAsync(Student student)
        {
            _students.Add(student);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Student student)
        {
            return Task.CompletedTask; 
        }

        public Task DeleteAsync(int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                _students.Remove(student);
            }
            return Task.CompletedTask;
        }
    }
}