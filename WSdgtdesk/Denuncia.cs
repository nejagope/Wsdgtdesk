using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSdgtdesk
{
    public class Denuncia
    {
        public int Id { get; set; }
        public string Detalles { get; set; }
        public string Direccion { get; set; }
        public string EmailUsuario { get; set; }
        public string TelefonoUsuario { get; set; }
        public string Acciones { get; set; }
        public DateTime FechaDenuncia { get; set; }
        public DateTime FechaAccion { get; set; }
        public bool Cerrada { get; set; }
        public DateTime FechaCierre { get; set; }
        public TipoDenuncia TipoDenuncia { get; set; }

    }
}