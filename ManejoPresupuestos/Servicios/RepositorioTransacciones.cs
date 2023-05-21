using System.Data;
using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios;

public interface IRepositorioTransacciones
{
    public Task CrearTransaccion(Transaccion transaccion);
}

public class RepositorioTransacciones : IRepositorioTransacciones
{
    private readonly string _connectionString;

    public RepositorioTransacciones(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task CrearTransaccion(Transaccion transaccion)
    {
        await using var connection = new SqlConnection(_connectionString);
        var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar", new {transaccion.UsuarioId,
            transaccion.FechaTransaccion, transaccion.Monto, transaccion.CategoriaId, transaccion.CuentaId,
            transaccion.Nota}, commandType:CommandType.StoredProcedure);
        transaccion.Id = id;
    }
}