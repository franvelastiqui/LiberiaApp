using AccesoDatos.Data;
using AccesoDatos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly LibreriaContexto contexto;
        internal DbSet<T> dbSet;

        public Repositorio(LibreriaContexto contexto)
        {
            this.contexto = contexto;
            dbSet = this.contexto.Set<T>();
        }

        public void Add(T item)
        {
            dbSet.Add(item);
        }

        public void Remove(T item)
        {
            dbSet.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            dbSet.RemoveRange(items);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filtro, string? propiedadesIncluidas = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filtro);

            if (!string.IsNullOrWhiteSpace(propiedadesIncluidas))
            {
                foreach (string prop in propiedadesIncluidas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    query = query.Include(prop);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAll(string? propiedadesIncluidas = null)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrWhiteSpace(propiedadesIncluidas))
            {
                foreach(string prop in propiedadesIncluidas.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    query = query.Include(prop);
                }
            }

            return await query.ToListAsync();
        }

        public IQueryable<T> GetAllQuery(string? propiedadesIncluidas = null)
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrWhiteSpace(propiedadesIncluidas))
            {
                foreach (string prop in propiedadesIncluidas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    query = query.Include(prop);
                }
            }

            return query.AsNoTracking();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filtro, string? propiedadesIncluidas = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filtro);

            if (!string.IsNullOrWhiteSpace(propiedadesIncluidas))
            {
                foreach (string prop in propiedadesIncluidas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    query = query.Include(prop);
                }
            }

            return await query.ToListAsync();

        }
    }
}
