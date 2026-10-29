using Moq;
using SistemaCore.Api.Modules.Inicio.Application;
using SistemaCore.Api.Modules.Inicio.Domain;
using SistemaCore.Api.Shared.Domain;
using SistemaCore.Api.Shared.Security;
using Xunit;

namespace SistemaCore.Tests.Inicio;

public class ResumenDiaServiceTests
{
    [Fact]
    public async Task DevuelveCeroYPendienteCuandoElDiaNoTieneMovimientosNiCierre()
    {
        var usuario = new Mock<ICurrentUser>();
        usuario.SetupGet(u => u.Nombre).Returns("Operador de ejemplo");
        usuario.SetupGet(u => u.Rol).Returns(Roles.Operador);

        var fecha = new Mock<IFechaOperativaProvider>();
        fecha.Setup(p => p.ObtenerVigenteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateOnly(2026, 11, 2));

        var movimientos = new Mock<IResumenMovimientosPort>();
        movimientos.Setup(p => p.ContarAsync(new DateOnly(2026, 11, 2), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var cierre = new Mock<IEstadoCierrePort>();
        cierre.Setup(p => p.ObtenerEstadoAsync(new DateOnly(2026, 11, 2), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Pendiente");

        var service = new ResumenDiaService(usuario.Object, fecha.Object, movimientos.Object, cierre.Object);
        ResumenDiaResponse resumen = await service.ObtenerAsync(CancellationToken.None);

        Assert.Equal("Operador de ejemplo", resumen.Nombre);
        Assert.Equal(Roles.Operador, resumen.Rol);
        Assert.Equal("2026-11-02", resumen.FechaOperativa);
        Assert.Equal(0, resumen.CantidadMovimientos);
        Assert.Equal("Pendiente", resumen.EstadoCierre);
    }

    [Fact]
    public async Task ConsultaLasFuentesEnCadaIngreso()
    {
        var usuario = new Mock<ICurrentUser>();
        var fecha = new Mock<IFechaOperativaProvider>();
        fecha.SetupSequence(p => p.ObtenerVigenteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateOnly(2026, 11, 2))
            .ReturnsAsync(new DateOnly(2026, 11, 3));

        var movimientos = new Mock<IResumenMovimientosPort>();
        movimientos.Setup(p => p.ContarAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        var cierre = new Mock<IEstadoCierrePort>();
        cierre.Setup(p => p.ObtenerEstadoAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("En curso");

        var service = new ResumenDiaService(usuario.Object, fecha.Object, movimientos.Object, cierre.Object);
        ResumenDiaResponse primero = await service.ObtenerAsync(CancellationToken.None);
        ResumenDiaResponse segundo = await service.ObtenerAsync(CancellationToken.None);

        Assert.Equal("2026-11-02", primero.FechaOperativa);
        Assert.Equal("2026-11-03", segundo.FechaOperativa);
        movimientos.Verify(p => p.ContarAsync(new DateOnly(2026, 11, 3), It.IsAny<CancellationToken>()), Times.Once);
    }
}
