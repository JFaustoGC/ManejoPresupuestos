using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios;

public interface IRepositorioCuentas
{
    Task Crear(Cuenta cuenta);
    Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
    public Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    public Task Actualizar(CuentaCreacionViewModel cuenta);
    public Task Borrar(int id);
}

public class RepositorioCuentas : IRepositorioCuentas
{
    private readonly string _connectionString;


    public RepositorioCuentas(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(Cuenta cuenta)
    {
        await using var connection = new SqlConnection(_connectionString);
        var id = await connection.QuerySingleAsync<int>(
            @"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance) VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance);
            SELECT SCOPE_IDENTITY();", cuenta);

        cuenta.Id = id;
    }

    public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Cuenta>(
            @"SELECT Cuentas.Id, Cuentas.Nombre, Balance, TC.Nombre AS TipoCuenta FROM Cuentas INNER JOIN TiposCuentas TC ON TC.Id = Cuentas.TipoCuentaId WHERE TC.UsuarioId = @UsuarioId ORDER BY TC.Orden",
            new { usuarioId });
    }

    public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Cuenta>(
            @"SELECT Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, TipoCuentaId FROM Cuentas INNER JOIN TiposCuentas TC ON TC.Id = Cuentas.TipoCuentaId WHERE TC.UsuarioId = @UsuarioId AND Cuentas.Id = @Id",
            new { id ,usuarioId });
    }

    public async Task Actualizar(CuentaCreacionViewModel cuenta)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"Update Cuentas
                                            SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId WHERE Id = @Id",
            cuenta);
    }

    public async Task Borrar(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"DELETE Cuentas WHERE Id = @Id", new {id});
    }
}