using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    // Define custom headers
    c.AddSecurityDefinition("X-Api-Key", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        Description = "API key required to access the endpoints"
    });

    c.AddSecurityDefinition("X-Client-Id", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Client-Id",
        Type = SecuritySchemeType.ApiKey,
        Description = "Client ID for identifying the application consuming the API"
    });

    // Apply them as global requirements
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "X-Api-Key" }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "X-Client-Id" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();