using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using POSSystem.Infrastructure.Data;
using POSSystem.Repository.BrandDefinition;
using POSSystem.Repository.CategoryDefinition;
using POSSystem.Repository.CustomerDefinition;
using POSSystem.Repository.GRNDefinition;
using POSSystem.Repository.ProductDefinition;
using POSSystem.Repository.PurchaseDefinition;
using POSSystem.Repository.SaleDefinition;
using POSSystem.Repository.SalesPersonDefinition;
using POSSystem.Repository.SalesReturnDefinition;
using POSSystem.Repository.SequenceDefinition;
using POSSystem.Repository.SupplierDefinition;
using POSSystem.Repository.UnitDefinition;
using POSSystem.Service.Auth;
using POSSystem.Service.BrandDefinition;
using POSSystem.Service.CategoryDefinition;
using POSSystem.Service.CustomerDefinition;
using POSSystem.Service.GRNDefinition;
using POSSystem.Service.ProductDefinition;
using POSSystem.Service.PurchaseDefinition;
using POSSystem.Service.SaleDefinition;
using POSSystem.Service.SaleReturnDefinition;
using POSSystem.Service.SalesPersonDefinition;
using POSSystem.Service.SequenceDefinition;
using POSSystem.Service.SupplierDefinition;
using POSSystem.Service.UnitDefinition;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ?????????????????????????????????????????????
// Database
// ?????????????????????????????????????????????
builder.Services.AddScoped<DatabaseConnection>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new DatabaseConnection(connectionString!);
});

// ?????????????????????????????????????????????
// Repositories & Services
// ?????????????????????????????????????????????
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISalesPersonRepository, SalesPersonRepository>();
builder.Services.AddScoped<ISalesPersonService, SalesPersonService>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IGRNRepository, GRNRepository>();
builder.Services.AddScoped<IGRNService, GRNService>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<ISalesReturnRepository, SalesReturnRepository>();
builder.Services.AddScoped<ISalesReturnService, SalesReturnService>();
builder.Services.AddScoped<ISequenceRepository, SequenceRepository>();
builder.Services.AddScoped<ISequenceService, SequenceService>();

// ?????????????????????????????????????????????
// Auth
// ?????????????????????????????????????????????
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<POSSystem.Repository.Auth.IAuthRepository, POSSystem.Repository.Auth.AuthRepository>();
builder.Services.AddScoped<POSSystem.Service.Auth.IAuthService, POSSystem.Service.Auth.AuthService>();

// ?????????????????????????????????????????????
// JWT Authentication  ?? NEW
// ?????????????????????????????????????????????
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing from configuration");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// ?????????????????????????????????????????????
// Controllers & Swagger
// ?????????????????????????????????????????????
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ?????????????????????????????????????????????
// CORS
// ?????????????????????????????????????????????
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("ReactPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();   // ?? MUST come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();