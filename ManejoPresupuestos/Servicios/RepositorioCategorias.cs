using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios;

public interface IRepositorioCategorias
{
    public Task Crear(Categoria categoria);
    public Task<IEnumerable<Categoria>> Obtener(int usuarioId);
    public Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId);
    public Task<Categoria> ObtenerPorId(int id, int usuarioId);
    public Task Actualizar(Categoria categoria);
    public Task Borrar(int id);
}

public class RepositorioCategorias : IRepositorioCategorias
{
    private readonly string _connectionString;

    public RepositorioCategorias(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task Crear(Categoria categoria)
    {
        await using var connection = new SqlConnection(_connectionString);
        var id = await connection.QuerySingleAsync<int>(
            @"INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId) Values (@Nombre, @TipoOperacionId, @UsuarioId);
            SELECT SCOPE_IDENTITY();", categoria);
        categoria.Id = id;
    }

    public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Categoria>("SELECT * FROM Categorias WHERE UsuarioId = @UsuarioId",
            new { usuarioId });
    }

    public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<Categoria>(
            "SELECT * FROM Categorias WHERE UsuarioId = @UsuarioId AND TipoOperacionId = @TipoOperacionId",
            new { usuarioId, tipoOperacionId });
    }

    public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Categoria>(
            "SELECT * FROM Categorias WHERE Id = @Id AND UsuarioId = @UsuarioId", new { id, usuarioId });
    }

    public async Task Actualizar(Categoria categoria)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Categorias SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId WHERE Id = @Id",
            categoria);
    }

    public async Task Borrar(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("DELETE Categorias WHERE Id = @Id", new { id });
    }
}