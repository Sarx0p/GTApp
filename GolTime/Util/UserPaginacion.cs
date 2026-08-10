namespace GolTime.Util
{
    public class UserPaginacion
    {
        public int TotalRegistros { get; private set; }
        public int PaginaActual { get; private set; }
        public int RegistrosPagina { get; private set; }
        public int TotalPaginas { get; private set; }
        public int PaginaInicio { get; private set; }
        public int UltimaPagina { get; private set; }
        public string Controlador { get; private set; }
        public string Accion { get; private set; }
        public int Salto { get; private set; }
        public int? RolId { get; private set; }
        public string? Estado { get; private set; }
        public string? Busqueda { get; private set; }

        public UserPaginacion()
        {
            Controlador = "User";
            Accion = "Index";
        }

        public UserPaginacion(
            int totalRegistros,
            int pagina,
            int registrosPagina = 5,
            string controlador = "User",
            string accion = "Index",
            int? rolId = null,
            string? estado = null,
            string? busqueda = null)
        {
            int totalPaginas = (int)Math.Ceiling((decimal)totalRegistros / (decimal)registrosPagina);
            int paginaActual = pagina < 1 ? 1 : pagina;

            if (totalPaginas > 0 && paginaActual > totalPaginas)
            {
                paginaActual = totalPaginas;
            }

            int paginaInicio = paginaActual - 5;
            int ultimaPagina = paginaActual + 4;
            int salto = (paginaActual - 1) * registrosPagina;

            if (paginaInicio <= 0)
            {
                ultimaPagina = ultimaPagina - (paginaInicio - 1);
                paginaInicio = 1;
            }

            if (ultimaPagina > totalPaginas)
            {
                ultimaPagina = totalPaginas;
                if (ultimaPagina > 10)
                {
                    paginaInicio = ultimaPagina - 9;
                }
            }

            TotalRegistros = totalRegistros;
            PaginaActual = paginaActual;
            RegistrosPagina = registrosPagina;
            TotalPaginas = totalPaginas;
            PaginaInicio = paginaInicio;
            UltimaPagina = ultimaPagina;
            Controlador = controlador;
            Accion = accion;
            Salto = salto;
            RolId = rolId;
            Estado = estado;
            Busqueda = busqueda;
        }
    }
}
