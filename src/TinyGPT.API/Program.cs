using Microsoft.AspNetCore.Mvc;
using TinyGPT.Application;
using TinyGPT.Application.Features.GenerateTextQuery;
using TinyGPT.Application.Features.TrainModelCommand;
using TinyGPT.Application.Extensions;
using TinyGPT.Infrastructure.Extensions;
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
    return Results.Ok(result);
});

app.MapGet(
    "/generate",
    async (
        [AsParameters] GenerateTextQuery query,
        [FromServices] OperationExecutor mediator,
        CancellationToken ct) =>
{
    var result = await mediator
        .ExecuteAsync<GenerateTextQuery, string>(query, ct);
    return Results.Ok(result);
});

app.Run();


