using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System.Net.Http.Json;
using System.Text.Json;
using Serilog.Debugging;
using Musaed.Core.Dtos;
using Serilog;

namespace Musaed.Infrastructure.Logging
{
    public class HttpSink : IBatchedLogEventSink
    {
        private readonly string _source;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _httpClientName;
        private HttpClient _httpClient; // Lazily initialized to avoid circular deps in serilogsetup
        private readonly string _bufferFilePath;

        public HttpSink(
            string source,
            string bufferFilePath,
            IHttpClientFactory httpClientFactory, 
            string httpClientName
            )
        {
            _bufferFilePath = bufferFilePath;
            _source = source;
            _httpClientFactory = httpClientFactory;
            _httpClientName = httpClientName;
        }
        private HttpClient GetHttpClient()
        {
            
            return _httpClient ??= _httpClientFactory.CreateClient(_httpClientName);
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            await TrySendBufferedLogAsync();

            var logEntries = batch.Select(LogEvent => new LogEntryDto
            {
                TimeStamp = LogEvent.Timestamp.UtcDateTime,
                Level = LogEvent.Level.ToString(),
                Message = LogEvent.RenderMessage(),
                Source = _source,
                Hostname = Environment.MachineName
            }).ToList();

            if (!logEntries.Any())
            {
                return;
            }

            var client = GetHttpClient();

            foreach (var logEntry in logEntries)
            {
                try
                {
                    var response = await client.PostAsJsonAsync("Logs", logEntry);
                    if (!response.IsSuccessStatusCode)
                    {
                        SelfLog.WriteLine($"Error sending a log to backend. Status : {response.StatusCode}. " +
                            $"Buffering the entire batch");
                        await WriteBatchToBufferAsync(logEntries);
                        return;
                    }

                }
                catch(Exception ex)
                {

                    SelfLog.WriteLine($"Error sending a log to backend. Status : {ex.Message}. " +
                        $"Buffering the entire batch");
                    await WriteBatchToBufferAsync(logEntries);
                    return;
                }

            }
        }


        private async Task WriteBatchToBufferAsync(IEnumerable<LogEntryDto> LogEntries)
        {
            try
            {
                var jsonEntries = LogEntries.Select(e => JsonSerializer.Serialize(e));
                await File.AppendAllLinesAsync(_bufferFilePath, jsonEntries);
            }
            catch(Exception ex)
            {
                SelfLog.WriteLine($"Failed to write the log batch to buffer file {_bufferFilePath}");
            }
        }

        public Task OnEmptyBatchAsync() => Task.CompletedTask;


        private async Task TrySendBufferedLogAsync()
        {
            if (!File.Exists(_bufferFilePath)) return;
            var tempBufferFilePath = _bufferFilePath + ".tmp";

            try
            {
                File.Move(_bufferFilePath, tempBufferFilePath);
                var bufferedLines = await File.ReadAllLinesAsync(tempBufferFilePath);
                
                if (!bufferedLines.Any()) {
                    File.Delete(tempBufferFilePath);
                    return;
                };

                SelfLog.WriteLine($"Attempting to send {bufferedLines.Length} log events.");
                var remainingLines = new List<string>(bufferedLines);

                foreach(var line in bufferedLines)
                {
                    try
                    {
                        var entry = JsonSerializer.Deserialize<LogEntryDto>(line);
                        if (entry == null) continue;

                        var client = GetHttpClient();

                        var response = await client.PostAsJsonAsync("Logs", entry);
                        if (response.IsSuccessStatusCode)
                        {
                            remainingLines.Remove(line);
                        }
                        else
                        {
                            SelfLog.WriteLine($"Failed to send buffer log. Status :{response.StatusCode}");

                            await File.WriteAllLinesAsync(_bufferFilePath, remainingLines);
                            File.Delete(tempBufferFilePath);
                            return;
                        }
                    }
                    catch(JsonException ex)
                    {
                        SelfLog.WriteLine($"skipping invalid buffer log line. Error :" +
                            $" {ex.Message}. Line: {line}");
                        remainingLines.Remove(line);                       
                    }
                    catch(Exception ex)
                    {
                        SelfLog.WriteLine($"An error occured while sending buffered log:{ex.Message}");
                        await File.WriteAllLinesAsync(_bufferFilePath, remainingLines);
                        File.Delete(tempBufferFilePath);
                        return;
                    }
                }

                File.Delete(tempBufferFilePath);
                SelfLog.WriteLine("Successfully sent all buffered logs.");


            }
            catch(Exception ex)
            {
                SelfLog.WriteLine($"An error occured while processing the buffer file:{ex.Message}");
                if (File.Exists(tempBufferFilePath) && !File.Exists(_bufferFilePath))
                    File.Move(tempBufferFilePath, _bufferFilePath);
            }
        }
    }
}
