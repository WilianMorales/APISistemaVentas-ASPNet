﻿using System;
using System.Collections.Generic;

namespace SistemaVenta.Model;

public partial class Rol
{
    public int IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly? FechaRegistro { get; set; }

    public virtual ICollection<MenuRol> MenuRols { get; set; } = new List<MenuRol>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
