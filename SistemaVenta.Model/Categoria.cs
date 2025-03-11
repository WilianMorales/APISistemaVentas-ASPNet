using System;
using System.Collections.Generic;

namespace SistemaVenta.Model;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? EsActivo { get; set; }

    public DateOnly? FechaRegistro { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
