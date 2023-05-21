﻿using AutoMapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Controllers;

public class CuentasController : Controller
{
    private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
    private readonly IservicioUsuarios _servicioUsuarios;
    private readonly IRepositorioCuentas _repositorioCuentas;
    private readonly IMapper _mapper;

    public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IservicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas, IMapper mapper)
    {
        _repositorioTiposCuentas = repositorioTiposCuentas;
        _servicioUsuarios = servicioUsuarios;
        _repositorioCuentas = repositorioCuentas;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var modelo = new CuentaCreacionViewModel();
        modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
        
        return View(modelo);
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var cuentasConTipoCuenta = await _repositorioCuentas.Buscar(usuarioId);

        var modelo = cuentasConTipoCuenta.GroupBy(x => x.TipoCuenta).Select(grupo => new IndiceCuentaViewModel()
        {
            TipoCuenta = grupo.Key,
            Cuentas = grupo.AsEnumerable()
        }).ToList();

        return View(modelo);

    }

    [HttpPost]
    public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

        if (tipoCuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        if (ModelState.IsValid)
        {
            cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(cuenta);
        }

        await _repositorioCuentas.Crear(cuenta);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var modelo = _mapper.Map<CuentaCreacionViewModel>(cuenta);

        modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var cuenta = await _repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);
        
        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        var tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);

        if (tipoCuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        await _repositorioCuentas.Actualizar(cuentaEditar);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Borrar(int id)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        return View(cuenta);
    }

    [HttpPost]
    public async Task<IActionResult> BorrarCuenta(int id)
    {
        var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
        var cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

        if (cuenta is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        await _repositorioCuentas.Borrar(id);
        return RedirectToAction("Index");
    }

    private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
    {
        var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);
return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
    }
}