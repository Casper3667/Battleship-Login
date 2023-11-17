using LoginServer.Services;

LoginServer.LoginStart.StartSequence(args); // Not entirely necessary but just to be safe it is done this way.
namespace LoginServer
{
    public static class LoginStart
    {
        public static void StartSequence(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddAuthentication().AddJwtBearer();

            builder.Services.AddDbContext<UserInteraction>();
            //builder.WebHost.UseUrls("http://localhost:80");

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<UserInteraction>();
                context.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
