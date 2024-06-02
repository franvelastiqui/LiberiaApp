using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ListaConPaginas<T> : List<T>
    {
        public int IndicePagina { get; private set; }
        public int TotalPaginas { get; private set; }

        private ListaConPaginas(List<T> items, int cantidad, int indicePagina, int tamanioPagina)
        {
            IndicePagina = indicePagina;
            TotalPaginas = (int)Math.Ceiling(cantidad / (double)tamanioPagina);

            this.AddRange(items);
        }

        public bool TienePaginaAnterior => IndicePagina > 1;
        public bool TienePaginaSiguiente => IndicePagina < TotalPaginas;

        public static async Task<ListaConPaginas<T>> CrearAsync(IQueryable<T> fuente, int indicePagina, int tamanioPagina)
        {
            int cantidad = await fuente.CountAsync();
            var items = await fuente.Skip((indicePagina - 1) * tamanioPagina).Take(tamanioPagina).ToListAsync();
            return new ListaConPaginas<T>(items, cantidad, indicePagina, tamanioPagina);
        }
    }
}
