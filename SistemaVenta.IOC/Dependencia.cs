﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.BLL.Servicios;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.Repositorios;
using SistemaVenta.Utility;

namespace SistemaVenta.IOC
{
    // Clase estática que proporciona métodos para inyectar dependencias en la capa de IOC (Inversión de Control).
    public static class Dependencia
    {
        public static void InyectarDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración de DbContext para la conexión a la base de datos utilizando SqlServer.
            services.AddDbContext<DbventaContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Registrar implementaciones para la interfaz IGenericRepository<> y IVentaRepository.
            // Esto permite que estas implementaciones se resuelvan automáticamente cuando se solicitan.
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVentaRepository, VentaRepository>();

            // Configurar AutoMapper usando el perfil definido en AutoMapperProfile.
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // Registro de servicios e interfaces mediante inyección de dependencias.
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IVentaService, VentaService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IMenuService, MenuService>();

        }
    }
}
