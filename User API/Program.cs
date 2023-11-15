global using JwtWebApiTutorial.Services.UserService;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Swashbuckle.AspNetCore.Filters;
using User_API.Models;
using User_API.Services;
using User_API.ServiceExtensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureSqlContext(builder.Configuration);

//builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<RepositoryContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


builder.Services.AddCors(options => options.AddPolicy("NgOrigins",
    policy => { policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader(); }));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NgOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();