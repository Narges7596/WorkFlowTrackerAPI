using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// Configure CORS policies (Cross-Origin Resource Sharing)
// This allows the API to be accessed from different origins, which is useful for development and production environments.
// In development, we allow localhost origins for Angular or React apps.
// In production, we restrict it to a specific domain. (We should change this to our production domain)
builder.Services.AddCors((options) =>
{
    options.AddPolicy("DevCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
    options.AddPolicy("ProdCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("https://myProductionSite.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure authentication using JWT (JSON Web Tokens)
string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;
SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString : ""));
TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
{
    IssuerSigningKey = tokenKey,
    ValidateIssuerSigningKey = false, // This is set to false for simplicity, but in production, you should validate the signing key.
    ValidateIssuer = false,
    ValidateAudience = false,
    ClockSkew = TimeSpan.Zero // This is to avoid issues with token expiration time
};
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = tokenValidationParameters;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseAuthentication(); // !!! This should be before UseAuthorization !!!
app.UseAuthorization(); ;

app.MapControllers();

app.Run();
