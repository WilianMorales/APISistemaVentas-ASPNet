using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;

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
        }
    }
}
