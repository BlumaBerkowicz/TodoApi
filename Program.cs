using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*") // או "*" עבור כל הדומיינים (לא מומלץ לייצור)
               .AllowAnyMethod() // או הגדר מתודות ספציפיות (GET, POST וכו')
               .AllowAnyHeader(); // או הגדר כותרות ספציפיות
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
        ServerVersion.Parse("8.0.40-mysql")
));
var app = builder.Build();
app.MapGet("/", async (ToDoDbContext toDoDbContext) => {
   var items = await toDoDbContext.Items.ToListAsync(); // קבל רק את הנתונים
    return items;
});
app.MapGet( "/item/{id}", async (ToDoDbContext toDoDbContext,int id) =>
{
   var item= await toDoDbContext.Items.FindAsync(id);
       return Results.Ok(item);
   }
);
app.MapPost("/item", async (ToDoDbContext toDoDbContext, [FromBody] Item item) => 
{
    toDoDbContext.Add(item);
        await toDoDbContext.SaveChangesAsync();
       return Results.Ok(item);
    });

app.MapPut("/item/{id}", async (ToDoDbContext toDoDbContext,[FromBody] Item item) =>{
 var existingItem = await toDoDbContext.Items.FindAsync(item.Id);
        Console.WriteLine("im here");
    if (existingItem == null)
    {
        Console.WriteLine("im here");
        return Results.NotFound(); 
    }
    existingItem.IsComplete = item.IsComplete; // Example: Update Name
    await toDoDbContext.SaveChangesAsync(); // Very Important! Save the changes
    return Results.NoContent(); 
    });
app.MapDelete("/item/{id}", async (ToDoDbContext toDoDbContext,[FromRoute] int id ) =>{
     var existingItem = await toDoDbContext.Items.FindAsync(id);
   if (existingItem == null)
    {
        return Results.NotFound(); // Or other appropriate response
    }
toDoDbContext.Remove(existingItem);
await toDoDbContext.SaveChangesAsync(); // Very Important! Save the changes
    return Results.NoContent(); 
});

app.MapMethods("/options-or-head", new[] { "OPTIONS", "HEAD" }, 
                          () => "This is an options or head request ");
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();