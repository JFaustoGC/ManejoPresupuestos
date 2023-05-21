using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Models;

public class TransaccionCreacionViewModel : Transaccion
{
    public IEnumerable<SelectListItem> Cuentas { get; set; }
    public IEnumerable<SelectListItem> Categorias { get; set; }
    [Display(Name = "Tipo Operacion")] public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;
}