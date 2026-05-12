namespace UeesDigital.Domain.Interfaces;

/// <summary>
/// Contrato del servicio de envío de correos electrónicos
/// </summary>
public interface IEmailService
{
    Task SendConfirmacionTramiteAsync(
        string destinatario,
        string nombreEstudiante,
        string codigoConfirmacion,
        string tipoTramite,
        string fecha,
        string hora
    );
}