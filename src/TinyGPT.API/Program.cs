using Microsoft.AspNetCore.Mvc;
using TinyGPT.Application;
using TinyGPT.Application.Features.TrainModelCommand;
using TinyGPT.Application.Extensions;
using TinyGPT.Infrastructure.Extensions;
using TinyGPT.Application.Features.GenerateTextCommand;
using TinyGPT.Application.Features.GenerateStreamCommand;
using System.Collections.Generic;
//using TinyGPT.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIfrastructure(builder.Configuration);
builder.Services.AddApplication();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost(
    "/train",
    async (
        [FromBody] TrainModelCommand cmd,
        [FromServices] OperationExecutor mediator,
        CancellationToken ct) =>
{
    var result = await mediator
        .ExecuteAsync<TrainModelCommand, string>(cmd, ct);
    return Results.Ok(new 
    { 
        status = "ok",
        message = result 
    });
});

app.MapPost(
    "/generate",
    async (
        [FromBody] GenerateTextCommand query,
        [FromServices] OperationExecutor mediator,
        CancellationToken ct) =>
{
    var result = await mediator
        .ExecuteAsync<GenerateTextCommand, string>(query, ct);
    return Results.Ok(result);
});

app.MapPost(
    "/generate/stream",
    async (
        HttpContext httpContext,
        [FromBody] GenerateStreamCommand query,
        [FromServices] OperationExecutor mediator,
        CancellationToken ct) => 
    {
        // ?? Important for streaming
        httpContext.Response.Headers.Add("Content-Type", "text/event-stream");

        var stream = await mediator
            .ExecuteAsync<GenerateStreamCommand, IAsyncEnumerable<string>>(
            query, ct);

        await foreach (var token in stream.WithCancellation(ct))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(new
            {
                model = "minigpt",
                token,
                done = false
            });

            await httpContext.Response.WriteAsync($"data: {json}\n\n");
            await httpContext.Response.Body.FlushAsync();
        }
        await httpContext.Response.WriteAsync("data: {\"done\":true}\n\n");
        //return Results.Ok(result);
    });

app.Run();


