using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using UeesDigital.Domain.Interfaces;

namespace UeesDigital.Infrastructure.Services;

/// <summary>
/// Servicio de correo usando Gmail SMTP
/// Configuración en appsettings.json → sección "Email"
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendConfirmacionTramiteAsync(
        string destinatario,
        string nombreEstudiante,
        string codigoConfirmacion,
        string tipoTramite,
        string fecha,
        string hora)
    {
        var emailConfig = _config.GetSection("Email");
        var host = emailConfig["SmtpHost"] ?? "smtp.gmail.com";
        var port = int.Parse(emailConfig["SmtpPort"] ?? "587");
        var usuario = emailConfig["Username"] ?? "";
        var password = emailConfig["Password"] ?? "";
        var remitente = emailConfig["From"] ?? usuario;
        var nombreApp = emailConfig["DisplayName"] ?? "UEES ID Digital";

        var asunto = $"✅ Confirmación de trámite — {codigoConfirmacion}";
        var cuerpo = BuildHtmlBody(
            nombreEstudiante, codigoConfirmacion, tipoTramite, fecha, hora, nombreApp);

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(usuario, password),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
        };

        using var mensaje = new MailMessage
        {
            From = new MailAddress(remitente, nombreApp),
            Subject = asunto,
            Body = cuerpo,
            IsBodyHtml = true,
        };
        mensaje.To.Add(destinatario);

        await client.SendMailAsync(mensaje);
    }

    private static string BuildHtmlBody(
        string nombre, string codigo, string tipo, string fecha, string hora, string appName)
    {
        return $"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
          <meta charset="UTF-8"/>
          <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
          <title>Confirmación de Trámite</title>
        </head>
        <body style="margin:0;padding:0;background:#f0f4fa;font-family:'Segoe UI',Arial,sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f0f4fa;padding:40px 20px;">
            <tr><td align="center">
              <table width="100%" style="max-width:560px;background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(10,42,114,.10);">

                <!-- Header -->
                <tr>
                  <td style="background:#0a2a72;padding:32px 40px;text-align:center;">
                    <div style="width:56px;height:56px;background:#f0c417;border-radius:50%;margin:0 auto 14px;display:flex;align-items:center;justify-content:center;">
                      <span style="font-size:28px;line-height:56px;">🎓</span>
                    </div>
                    <h1 style="color:#ffffff;margin:0;font-size:22px;font-weight:900;">{appName}</h1>
                    <p style="color:#a8c4f0;margin:6px 0 0;font-size:13px;">Sistema de Carné Universitario</p>
                  </td>
                </tr>

                <!-- Cuerpo -->
                <tr>
                  <td style="padding:36px 40px;">
                    <p style="color:#0a2a72;font-size:16px;font-weight:700;margin:0 0 8px;">
                      Hola, {nombre} 👋
                    </p>
                    <p style="color:#6a8ecc;font-size:14px;margin:0 0 28px;">
                      Tu trámite ha sido registrado exitosamente. Aquí están los detalles:
                    </p>

                    <!-- Tarjeta de datos -->
                    <table width="100%" cellpadding="0" cellspacing="0"
                      style="background:#f8fafd;border-radius:12px;border:1px solid #d0dcf4;overflow:hidden;margin-bottom:28px;">
                      <tr>
                        <td style="padding:20px 24px;">
                          <table width="100%" cellpadding="6" cellspacing="0">
                            <tr>
                              <td style="font-size:11px;font-weight:800;color:#6a8ecc;text-transform:uppercase;letter-spacing:.8px;width:140px;">
                                Código
                              </td>
                              <td style="font-size:15px;font-weight:900;color:#1344a8;">
                                {codigo}
                              </td>
                            </tr>
                            <tr>
                              <td style="border-top:1px solid #e8eef8;font-size:11px;font-weight:800;color:#6a8ecc;text-transform:uppercase;letter-spacing:.8px;">
                                Tipo de trámite
                              </td>
                              <td style="border-top:1px solid #e8eef8;font-size:14px;font-weight:700;color:#0a2a72;">
                                {tipo}
                              </td>
                            </tr>
                            <tr>
                              <td style="border-top:1px solid #e8eef8;font-size:11px;font-weight:800;color:#6a8ecc;text-transform:uppercase;letter-spacing:.8px;">
                                Fecha
                              </td>
                              <td style="border-top:1px solid #e8eef8;font-size:14px;font-weight:700;color:#0a2a72;">
                                {fecha}
                              </td>
                            </tr>
                            <tr>
                              <td style="border-top:1px solid #e8eef8;font-size:11px;font-weight:800;color:#6a8ecc;text-transform:uppercase;letter-spacing:.8px;">
                                Hora
                              </td>
                              <td style="border-top:1px solid #e8eef8;font-size:14px;font-weight:700;color:#0a2a72;">
                                {hora}
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </table>

                    <!-- Recordatorio -->
                    <table width="100%" cellpadding="0" cellspacing="0"
                      style="background:#fffbe6;border:1px solid #f0c417;border-radius:10px;margin-bottom:28px;">
                      <tr>
                        <td style="padding:14px 18px;">
                          <p style="margin:0;font-size:13px;color:#8a6000;font-weight:600;">
                            ⏰ <strong>Recuerda</strong> presentarte puntualmente con
                            este código de confirmación el día de tu cita.
                          </p>
                        </td>
                      </tr>
                    </table>

                    <p style="color:#6a8ecc;font-size:13px;margin:0;">
                      Si no realizaste esta solicitud, ignora este correo o contáctanos.
                    </p>
                  </td>
                </tr>

                <!-- Footer -->
                <tr>
                  <td style="background:#f8fafd;border-top:1px solid #d0dcf4;padding:20px 40px;text-align:center;">
                    <p style="margin:0;font-size:12px;color:#6a8ecc;">
                      © {DateTime.Now.Year} {appName} · Universidad Evangélica de El Salvador
                    </p>
                  </td>
                </tr>

              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;
    }
}