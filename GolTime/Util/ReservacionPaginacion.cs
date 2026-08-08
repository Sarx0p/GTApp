using GolTime.Models;
using Microsoft.EntityFrameworkCore;

namespace GolTime.Util
{

    public class ReservacionPaginacion
    {
        public List<Reservacion> Items { get; set; } = new();

        public int PaginaActual { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }

        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

        public static async Task<ReservacionPaginacion> CrearAsync(
            IQueryable<Reservacion> query,
            int paginaActual,
            int tamanoPagina = 5)
        {
            if (paginaActual < 1)
            {
                paginaActual = 1;
            }

            var totalRegistros = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanoPagina);

            var items = await query
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new ReservacionPaginacion
            {
                Items = items,
                PaginaActual = paginaActual,
                TamanoPagina = tamanoPagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas
            };
        }
    }
}