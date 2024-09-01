using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Infrastructure.WebSocket;
using WebSocket = System.Net.WebSockets.WebSocket;

public class ConnectionManager
{
    private readonly ConcurrentDictionary<string, List<WebSocket>> _sockets = new();
    
    private static string NormalizeId(string id) => id.ToLower();

    public ConcurrentDictionary<string, List<WebSocket>> GetAll()
    {
        return _sockets;
    }

    public IEnumerable<WebSocket>? GetSockets(string id)
    {
        return _sockets.GetValueOrDefault(NormalizeId(id));
    } 
    
    public string? GetId(WebSocket socket)
    {
        var id = _sockets.FirstOrDefault(x => x.Value.Any(s => s == socket)).Key;
        return string.IsNullOrEmpty(id) ? null : NormalizeId(id);
    }

    public void AddSocket(string id, WebSocket socket)
    {
        var existed = _sockets.GetValueOrDefault(id);
        
        if (existed is null)
        {
            _sockets[NormalizeId(id)] = [socket];
            return;
        }
        
        existed.Add(socket);
    }
    
    public async Task RemoveSocketAsync(WebSocket socket)
    {
        foreach (var pair in _sockets)
        {
            var removed = pair.Value.Remove(socket);
            
            if (removed)
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "The server closes this connection",
                    CancellationToken.None);
            }

            if (pair.Value.Count == 0) _sockets.TryRemove(pair);
        }
    }
}