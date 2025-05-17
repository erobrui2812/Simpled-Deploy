using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Channels;

namespace Simpled.Services
{
    public class SseInvitationBroadcastService
    {
        private readonly ConcurrentDictionary<string, List<ChannelWriter<string>>> _userStreams = new();

        public void Register(string email, ChannelWriter<string> writer)
        {
            var list = _userStreams.GetOrAdd(email, _ => new List<ChannelWriter<string>>());
            lock (list)
            {
                list.Add(writer);
            }
        }

        public void Unregister(string email, ChannelWriter<string> writer)
        {
            if (_userStreams.TryGetValue(email, out var list))
            {
                lock (list)
                {
                    list.Remove(writer);
                }
            }
        }

        public async Task BroadcastInvitationAsync(string email, object invitationDto, string type)
        {
            if (_userStreams.TryGetValue(email, out var list))
            {
                var payload = JsonSerializer.Serialize(new { type, data = invitationDto });
                List<ChannelWriter<string>> toRemove = new();
                lock (list)
                {
                    foreach (var writer in list)
                    {
                        if (!writer.TryWrite(payload))
                        {
                            toRemove.Add(writer);
                        }
                    }
                    foreach (var wr in toRemove)
                        list.Remove(wr);
                }
            }
        }
    }
} 