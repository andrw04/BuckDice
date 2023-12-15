using BuckDice.Server;
using BuckDice.Server.Hubs;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GameManager>();

builder.Services.AddSignalR();

builder.Services.AddCors();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseResponseCompression();

app.MapHub<GameHub>("/buckdice");

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.Run();
