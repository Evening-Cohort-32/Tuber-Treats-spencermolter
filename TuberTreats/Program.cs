using TuberTreats.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here
List<TuberDriver> tuberDrivers = new List<TuberDriver>()
{
    new TuberDriver { Id = 1, Name = "Carroll Shelby" },
    new TuberDriver { Id = 2, Name = "Ken Miles" },
    new TuberDriver { Id = 3, Name = "Max Verstappen" }
};

List<Customer> customers = new List<Customer>()
{
    new Customer { Id = 1, Name = "James Bond", Address = "12 Tatooine Way" },
    new Customer { Id = 2, Name = "Jeremy Clarkson", Address = "44 Alderaan Blvd" },
    new Customer { Id = 3, Name = "Richard Hammond", Address = "77 Corellia St" },
    new Customer { Id = 4, Name = "James May", Address = "101 Cloud City Ct" },
    new Customer { Id = 5, Name = "Billy Idol", Address = "300 Jedi Temple Rd" }
};

List<Topping> toppings = new List<Topping>()
{
    new Topping { Id = 1, Name = "Cheese" },
    new Topping { Id = 2, Name = "Bacon" },
    new Topping { Id = 3, Name = "Spinach" },
    new Topping { Id = 4, Name = "Pineapple" },
    new Topping { Id = 5, Name = "Butter" }
};

List<TuberOrder> tuberOrders = new List<TuberOrder>()
{
    new TuberOrder
    {
        Id = 1,
        OrderPlacedOnDate = new DateTime(2026, 10, 1),
        CustomerId = 1,
        TuberDriverId = 1,
        DeliveredOnDate = new DateTime(2026, 10, 1)
    },
    new TuberOrder
    {
        Id = 2,
        OrderPlacedOnDate = new DateTime(2026, 10, 2),
        CustomerId = 3,
        TuberDriverId = 2,
        DeliveredOnDate = null
    },
    new TuberOrder
    {
        Id = 3,
        OrderPlacedOnDate = new DateTime(2026, 10, 2),
        CustomerId = 5,
        TuberDriverId = null,
        DeliveredOnDate = null
    }
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping { Id = 1, TuberOrderId = 1, ToppingId = 1 },
    new TuberTopping { Id = 2, TuberOrderId = 1, ToppingId = 3 },
    new TuberTopping { Id = 3, TuberOrderId = 2, ToppingId = 2 }
};

app.MapGet("/toppings", () => {
    return Results.Ok(toppings);
});

app.MapGet("/toppings/{id}", (int id) =>
{
    Topping topping = toppings.FirstOrDefault(t => t.Id == id);
    if (topping == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(topping);
});



app.MapGet("/tuberorders", () => {
    return Results.Ok(tuberOrders);
});

app.MapGet("/tuberorders/{id}", (int id) => {
    TuberOrder order = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound();
    }

    order.Customer = customers.FirstOrDefault(c => c.Id == order.CustomerId);
    if (order.TuberDriverId != null)
    {
        order.TuberDriver = tuberDrivers.FirstOrDefault(d => d.Id == order.TuberDriverId);
    }

    List<TuberTopping> orderTuberToppings = tuberToppings.Where(tt => tt.TuberOrderId == id).ToList();
    order.Toppings = orderTuberToppings.Select(tt => toppings.First(t => t.Id == tt.ToppingId)).ToList();

    return Results.Ok(order);
});



app.MapPost("/tuberorders", (TuberOrder order) => {
    order.Id = tuberOrders.Count > 0 ? tuberOrders.Max(o => o.Id) + 1 : 1;
    order.OrderPlacedOnDate = DateTime.Now;
    tuberOrders.Add(order);

    return Results.Created($"/tuberorders/{order.Id}", order);
});


app.MapPut("/tuberorders/{id}", (int id, TuberOrder order) => {
    TuberOrder orderToUpdate = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (orderToUpdate == null)
    {
        return Results.NotFound();
    }

    orderToUpdate.TuberDriverId = order.TuberDriverId;
    return Results.NoContent();
});


app.MapPost("/tuberorders/{id}/complete", (int id) => {
    TuberOrder orderToComplete = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (orderToComplete == null)
    {
        return Results.NotFound();
    }

    orderToComplete.DeliveredOnDate = DateTime.Now;
    return Results.NoContent();
});


app.MapGet("/tubertoppings", () => {
    return Results.Ok(tuberToppings);
});

app.MapPost("/tubertoppings", (TuberTopping tuberTopping) => {
    tuberTopping.Id = tuberToppings.Count > 0 ? tuberToppings.Max(tt => tt.Id) + 1 : 1;
    tuberToppings.Add(tuberTopping);

    return Results.Created($"/tubertoppings/{tuberTopping.Id}", tuberTopping);
});



app.MapDelete("/tubertoppings/{id}", (int id) => {
    TuberTopping tuberTopping = tuberToppings.FirstOrDefault(tt => tt.Id == id);
    if (tuberTopping == null)
    {
        return Results.NotFound();
    }

    tuberToppings.Remove(tuberTopping);
    return Results.NoContent();
});


app.MapGet("/customers", () => {
    return Results.Ok(customers);
});

app.MapGet("/customers/{id}", (int id) => {
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }

    customer.TuberOrders = tuberOrders.Where(o => o.CustomerId == id).ToList();
    return Results.Ok(customer);
});

app.MapPost("/customers", (Customer customer) => {
    customer.Id = customers.Count > 0 ? customers.Max(c => c.Id) + 1 : 1;
    customers.Add(customer);

    return Results.Created($"/customers/{customer.Id}", customer);
});

app.MapDelete("/customers/{id}", (int id) => {
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }

    customers.Remove(customer);
    return Results.NoContent();
});


app.MapGet("/tuberdrivers", () => {
    return Results.Ok(tuberDrivers);
});

app.MapGet("/tuberdrivers/{id}", (int id) => {
    TuberDriver driver = tuberDrivers.FirstOrDefault(d => d.Id == id);
    if (driver == null)
    {
        return Results.NotFound();
    }

    driver.TuberDeliveries = tuberOrders.Where(o => o.TuberDriverId == id).ToList();
    return Results.Ok(driver);
});

app.Run();
//don't touch or move this!
public partial class Program { }