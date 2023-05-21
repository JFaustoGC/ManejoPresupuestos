using System.Data;
using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios;

public interface IRepositorioTiposCuentas
{
    public Task Crear(TipoCuenta tipoCuenta);
    public Task<bool> Existe(string? nombre, int usuarioId);
    public Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
    public Task Actualizar(TipoCuenta tipoCuenta);
    public Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
    public Task Borrar(int id);
    public Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados);
}

public class RepositorioTiposCuentas : IRepositorioTiposCuentas
{
    private readonly string _connectionString;

    public RepositorioTiposCuentas(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(TipoCuenta tipoCuenta)
    {
        await using var connection = new SqlConnection(_connectionString);
        var id = await connection.QuerySingleAsync<int>(
            "TiposCuentas_Insertar", new { usuarioId = tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Existe(string? nombre, int usuarioId)
    {
        await using var connection = new SqlConnection(_connectionString);
        var existe = await connection.QueryFirstOrDefaultAsync<int>(
            $"SELECT 1 FROM TiposCuentas WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId", new { nombre, usuarioId });
        return existe == 1;
    }

    public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<TipoCuenta>(
            @"Select Id, Nombre, Orden FROM TiposCuentas WHERE UsuarioId = @UsuarioId ORDER BY Orden",
            new { usuarioId });
    }

    public async Task Actualizar(TipoCuenta tipoCuenta)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Nombre = @Nombre WHERE Id = @Id", tipoCuenta);
    }

    public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(
            @"SELECT Id, Nombre, Orden FROM TiposCuentas WHERE Id=@Id AND UsuarioId=@UsuarioId", new { id, usuarioId });
    }

    public async Task Borrar(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id=@Id", new { id });
    }

    public async Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Orden=@Orden WHERE Id=@Id", tiposCuentasOrdenados);
    }
}