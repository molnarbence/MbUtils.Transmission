using System.Text.Json;
using FakeApi;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR((config) => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
var jsonDeserializerOptions = new JsonSerializerOptions
{
   PropertyNameCaseInsensitive = true
};


app.MapPost("/transmission/rpc", async (HttpRequest request, IMediator mediator) =>
{
   using var streamReader = new StreamReader(request.Body);
   var bodyAsString = await streamReader.ReadToEndAsync();

   RpcRequest rpcRequest = JsonSerializer.Deserialize<RpcRequest>(bodyAsString, jsonDeserializerOptions) ?? throw new ApplicationException("Could not deserialize request");

   IMinimalApiRequest mediatorRequest = rpcRequest.Method switch
   {
      "torrent-get" => Deserialize<TorrentGetRequest>(),
      "torrent-start" => Deserialize<TorrentStartRequest>(),
      "torrent-add" => Deserialize<TorrentAddRequest>(),
      "torrent-stop" => Deserialize<TorrentStopRequest>(),
      "torrent-remove" => Deserialize<TorrentRemoveRequest>(),
      _ => throw new NotImplementedException()
   };

   return await mediator.Send(mediatorRequest);

   IMinimalApiRequest Deserialize<T>() where T : IMinimalApiRequest
   {
      return JsonSerializer.Deserialize<T>(bodyAsString, jsonDeserializerOptions) ?? throw new ApplicationException();
   }
});

app.MapGet("/", () => Results.Ok());

app.Run();