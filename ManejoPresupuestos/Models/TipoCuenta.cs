using System.ComponentModel.DataAnnotations;
using ManejoPresupuestos.Validaciones;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuestos.Models;

public class TipoCuenta
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [PrimeraLetraMayuscula]
    [Remote(action: "VerificarExisteTipoCuenta", controller:"TiposCuentas")]
    public string? Nombre { get; set; }
    public int UsuarioId { get; set; }
    public int Orden { get; set; }  
}