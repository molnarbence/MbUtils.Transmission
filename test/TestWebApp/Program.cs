using MbUtils.Transmission;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransmissionClient(builder.Configuration.GetSection("Transmission"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/torrents", async (ITransmissionClient transmissionClient) => { 
   var torrents = await transmissionClient.GetTorrentsAsync();
   return Results.Ok(torrents);
});

app.Run();
