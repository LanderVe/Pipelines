using System.IO.Pipelines;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/", async context =>
{
    PipeWriter pipeWriter = context.Response.BodyWriter;

    string content = "this is the content";
    var memory = pipeWriter.GetMemory(512);
    Encoding.UTF8.GetBytes(content, memory.Span);
    pipeWriter.Advance(content.Length);
    await pipeWriter.FlushAsync();
});

await app.RunAsync();