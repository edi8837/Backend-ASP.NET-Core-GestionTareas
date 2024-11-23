using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace WebApiPerson.Controllers
{
    [ApiController]
    [Route("ws")]  // Ruta de la API WebSocket, accediendo a través de "http://localhost:<puerto>/ws"
    public class WebSocketController : ControllerBase
    {
        // Método GET que maneja las solicitudes WebSocket
        [HttpGet]
        public async Task Get()
        {
            // Verifica si la solicitud es un WebSocket
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                // Si la solicitud es WebSocket, acepta la conexión
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                // Inicia la comunicación con el cliente WebSocket
                await HandleWebSocketCommunication(webSocket);
            }
            else
            {
                // Si no es una solicitud WebSocket, retorna un código de error 400 (solicitud incorrecta)
                HttpContext.Response.StatusCode = 400;
            }
        }

        // Método que maneja la comunicación WebSocket entre el servidor y el cliente
        private async Task HandleWebSocketCommunication(WebSocket webSocket)
        {
            // Define un buffer para recibir datos
            var buffer = new byte[1024 * 4];

            try
            {
                // Recibe el primer mensaje del WebSocket
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                // Mientras el WebSocket no cierre la conexión
                while (!result.CloseStatus.HasValue)
                {
                    // Convierte el mensaje recibido en un string
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Message received holaS: {message}");  // Imprime el mensaje recibido en consola

                    // Respuesta del servidor: "Mensaje recibido" junto con la fecha y hora
                    var responseMessage = "Message received at " + DateTime.Now;
                    var responseBuffer = Encoding.UTF8.GetBytes(responseMessage);

                    // Envía el mensaje de vuelta al cliente
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(responseBuffer),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );

                    // Recibe el siguiente mensaje
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");  // Manejo de errores
            }
            finally
            {
                // Si el WebSocket no fue cerrado correctamente, se asegura de cerrarlo
                if (webSocket.State != WebSocketState.CloseReceived)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Error occurred", CancellationToken.None);
                }
            }
        }
    }
}
