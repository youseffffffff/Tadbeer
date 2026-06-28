using BackEndApp.BusinessLogic;
using BackEndApp.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IDatabaseConnectionService, DatabaseConnectionService>();
builder.Services.AddScoped<UsersDataAccess>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FamiliesDataAccess>();
builder.Services.AddScoped<FamilyService>();
builder.Services.AddScoped<FamilyBudgetsDataAccess>();
builder.Services.AddScoped<FamilyBudgetService>();
builder.Services.AddScoped<BudgetPeriodsDataAccess>();
builder.Services.AddScoped<BudgetPeriodService>();
builder.Services.AddScoped<BudgetAllocationsDataAccess>();
builder.Services.AddScoped<BudgetAllocationService>();
builder.Services.AddScoped<ExpenseCategoriesDataAccess>();
builder.Services.AddScoped<ExpenseCategoryService>();
builder.Services.AddScoped<ExpensesDataAccess>();
builder.Services.AddScoped<ExpenseService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
