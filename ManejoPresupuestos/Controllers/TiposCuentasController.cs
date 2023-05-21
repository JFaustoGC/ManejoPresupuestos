using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuestos.Controllers;

public class TiposCuentasController : Controller
{
    private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
    private readonly IservicioUsuarios _servicioUsuarios;

    public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IservicioUsuarios servicioUsuarios)
    {
        _repositorioTiposCuentas = repositorioTiposCuentas;
        _servicioUsuarios = servicioUsuarios;
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

        if (tipoCuenta == null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        return View(tipoCuenta);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var tipoCuentaExiste = await _repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

        if (tipoCuentaExiste == null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        await _repositorioTiposCuentas.Actualizar(tipoCuenta);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);
        return View(tiposCuentas);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
    {
        if (!ModelState.IsValid) return View(tipoCuenta);

        tipoCuenta.UsuarioId = _servicioUsuarios.ObtenerUsuarioId();

        var yaExisteTipoCuenta = await _repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

        if (yaExisteTipoCuenta)
        {
            ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");
        }
        
        await _repositorioTiposCuentas.Crear(tipoCuenta);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Borrar(int id)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

        if (tipoCuenta == null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        return View(tipoCuenta);
    }

    public async Task<IActionResult> BorrarTipoCuenta(int id)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

        if (tipoCuenta == null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        await _repositorioTiposCuentas.Borrar(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var yaExisteTipoCuenta = await _repositorioTiposCuentas.Existe(nombre, usuarioId);

        return yaExisteTipoCuenta ? Json($"El nombre {nombre} ya existe") : Json(true);
    }

    [HttpPost]
    public async Task<IActionResult> Ordenar([FromBody] int[] ids)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);
        var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

        var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

        if (idsTiposCuentasNoPertenecenAlUsuario.Any())
        {
            return Forbid();
        }

        var tiposCuentasOrdenados = ids.Select((valor, indice) => new TipoCuenta() { Id = valor, Orden = indice + 1 })
            .AsEnumerable();

        await _repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);
        
        return Ok();
    }
}