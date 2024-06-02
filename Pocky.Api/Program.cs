using Pocky.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(op =>
      op.UseSqlServer(builder.Configuration.GetConnectionString("myCon")));
// ›Ì «·«’œ«— 8  ÷Ì› ›ﬁÿ «·«‰ Ì Ì »’œ—Ì‰ ›ﬁÿ 
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
// ›Ì «·«’œ«— 8  ÷Ì› ›ﬁÿ «·«‰ Ì Ì »’œ—Ì‰ ›ﬁÿ ÊÂÊ —Õ Ì÷Ì› ﬂ· «·«‰œ»ÊÌ‰ 
app.MapIdentityApi<IdentityUser>();

app.MapControllers();

app.Run();
