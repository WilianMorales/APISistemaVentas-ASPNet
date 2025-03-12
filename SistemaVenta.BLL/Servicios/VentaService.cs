using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaVenta.BLL.Servicios
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepository;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepository,
            IGenericRepository<DetalleVenta> detalleVentaRepository,
            IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _mapper = mapper;
        }

        // Método asincrónico que registra una venta.
        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await _ventaRepository.Registrar(_mapper.Map<Venta>(modelo));

                if (ventaGenerada.IdVenta == 0)
                    throw new TaskCanceledException("No se pudo registrar.");

                return _mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch
            {
                throw;
            }
        }

        // Método que proporciona un historial de ventas según los parámetros de búsqueda.
        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            // Consulta inicial que incluye detalles y productos relacionados con las ventas.
            IQueryable<Venta> query = await _ventaRepository.Consultar();

            // Lista donde se almacenarán los resultados de la consulta.
            var ListaReultado = new List<Venta>();

            try
            {
                // Verifica el criterio de búsqueda y realiza la consulta correspondiente.
                if (buscarPor == "fecha")
                {
                    DateTime fecha_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                    DateTime fecha_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

                    // Realiza la consulta filtrando por el rango de fechas.
                    ListaReultado = await query.Where(v =>
                       v.FechaRegistro.Value.Date >= fecha_Inicio.Date &&
                       v.FechaRegistro.Value.Date <= fecha_Fin.Date
                    ).Include(dv => dv.DetalleVenta)
                     .ThenInclude(p => p.IdProductoNavigation)
                     .ToListAsync();
                }
                else
                {
                    // Realiza la consulta filtrando por el número de venta.
                    ListaReultado = await query.Where(v =>
                       v.NumeroDocumento == numeroVenta
                    ).Include(dv => dv.DetalleVenta)
                     .ThenInclude(p => p.IdProductoNavigation)
                     .ToListAsync();
                }
            }
            catch
            {
                throw;
            }

            // Mapea la lista de ventasResultado a una lista de objetos VentaDTO y la devuelve.
            return _mapper.Map<List<VentaDTO>>(ListaReultado);
        }

        // Método Reporte recupera datos relacionados con ventas en un rango de fechas dado.
        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            // Se inicia con una consulta de DetalleVenta, que representa los detalles de ventas.
            IQueryable<DetalleVenta> query = await _detalleVentaRepository.Consultar();
            var ListaResultado = new List<DetalleVenta>();

            try
            {
                // Convertir las cadenas de fecha a objetos DateTime utilizando un formato específico.
                DateTime fecha_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                DateTime fecha_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

                // Se ejecuta la consulta, se incluye las relaciones de Producto y Venta, y se filtra por el rango de fechas.
                ListaResultado = await query
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => 
                       dv.IdVentaNavigation.FechaRegistro.Value.Date >= fecha_Inicio.Date &&
                       dv.IdVentaNavigation.FechaRegistro.Value.Date <= fecha_Fin.Date
                    ).ToListAsync();
            }
            catch
            {
                throw;
            }

            // Mapea los resultados a la clase DTO antes de devolverlos.
            return _mapper.Map<List<ReporteDTO>>(ListaResultado);
        }
    }
}
