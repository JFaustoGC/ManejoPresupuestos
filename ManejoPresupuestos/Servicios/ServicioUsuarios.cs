namespace ManejoPresupuestos.Servicios;

public interface IservicioUsuarios
{
    public int ObtenerUsuarioId();
}

public class ServicioUsuarios : IservicioUsuarios
{
    public int ObtenerUsuarioId()
    {
        return 1;
    }
}