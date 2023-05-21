using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Validaciones;

public class PrimeraLetraMayusculaAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var primeraLetra = value.ToString()?[0].ToString();

        return primeraLetra != primeraLetra?.ToUpper() ? new ValidationResult("La primera letra debe ser mayuscula") : ValidationResult.Success;
    }
}