using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication().AddBearerToken();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAuthorizationHandler, AlwaysFailingAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


internal sealed class FailAlwaysRequirement : IAuthorizationRequirement
{
    public static readonly FailAlwaysRequirement Instance = new FailAlwaysRequirement();

    private FailAlwaysRequirement() { }
}

internal sealed class FailAlwaysAttribute : Attribute, IAuthorizationRequirementData
{
    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return FailAlwaysRequirement.Instance;
    }
}

internal sealed class AlwaysFailingAuthorizationHandler : AuthorizationHandler<FailAlwaysRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FailAlwaysRequirement requirement)
    {
        context.Fail();

        return Task.CompletedTask;
    }
}