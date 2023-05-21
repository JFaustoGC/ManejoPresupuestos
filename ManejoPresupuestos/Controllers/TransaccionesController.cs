using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Controllers;

public class TransaccionesController : Controller
{
    private readonly IservicioUsuarios _servicioUsuarios;
    private readonly IRepositorioCuentas _repositorioCuentas;
    private readonly IRepositorioCategorias _repositorioCategorias;
    private readonly IRepositorioTransacciones _repositorioTransacciones;

    public TransaccionesController(IservicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias, IRepositorioTransacciones repositorioTransacciones)
    {
        _servicioUsuarios = servicioUsuarios;
        _repositorioCuentas = repositorioCuentas;
        _repositorioCategorias = repositorioCategorias;
        _repositorioTransacciones = repositorioTransacciones;
    }

    public async Task<IActionResult> Crear()
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var modelo = new TransaccionCreacionViewModel();
        modelo.Cuentas = await ObtenerCuentas(usuarioId);
        modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

        if (!ModelState.IsValid)
        {
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);
        }

        var cuenta = await _repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var categoria = await _repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);

        if (categoria is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        modelo.UsuarioId = usuarioId;

        if (modelo.TipoOperacionId == TipoOperacion.Gasto)
        {
            modelo.Monto *= -1;
        }

        await _repositorioTransacciones.CrearTransaccion(modelo);
        return RedirectToAction("Index");
    }

    private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
    {
        var cuentas = await _repositorioCuentas.Buscar(usuarioId);
        return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
    }

    private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, 
        TipoOperacion tipoOperacion)
    {
        var categorias = await _repositorioCategorias.Obtener(usuarioId, tipoOperacion);
        return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
    }

    [HttpPost]
    public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
        return Ok(categorias);
    }
}