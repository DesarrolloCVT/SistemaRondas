using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVTSistemaRondas.Models
{
    internal class UsuarioClass
    {
        public int? IdUsuario { get; set; }
        public string? UsuarioSistema { get; set; }
        public string? NombreUsuario { get; set; }
        public int IdPerfilMovile { get; set; }
    }
}
