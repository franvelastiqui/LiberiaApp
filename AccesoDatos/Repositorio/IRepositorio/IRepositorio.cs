using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(string? propiedadesIncluidas = null);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filtro, string? propiedadesIncluidas = null);
        IQueryable<T> GetAllQuery(string? propiedadesIncluidas = null);
        Task<T> Get(Expression<Func<T, bool>> filtro, string? propiedadesIncluidas = null);
        void Add(T item);
        void Remove(T item);
        void RemoveRange(IEnumerable<T> items);
    }
}
