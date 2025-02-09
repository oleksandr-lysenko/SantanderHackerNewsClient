var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5501);
});

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<HackerNewsClient.Services.HackerNewsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackerNewsAPI Client V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
