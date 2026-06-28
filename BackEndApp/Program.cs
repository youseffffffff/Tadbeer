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
builder.Services.AddScoped<SavingGoalsDataAccess>();
builder.Services.AddScoped<SavingGoalService>();
builder.Services.AddScoped<GoalContributionsDataAccess>();
builder.Services.AddScoped<GoalContributionService>();
builder.Services.AddScoped<ChallengesDataAccess>();
builder.Services.AddScoped<ChallengeService>();
builder.Services.AddScoped<ChallengeEvaluationsDataAccess>();
builder.Services.AddScoped<ChallengeEvaluationService>();
builder.Services.AddScoped<AchievementDefinitionsDataAccess>();
builder.Services.AddScoped<AchievementDefinitionService>();
builder.Services.AddScoped<UserAchievementsDataAccess>();
builder.Services.AddScoped<UserAchievementService>();
builder.Services.AddScoped<RewardsDataAccess>();
builder.Services.AddScoped<RewardService>();
builder.Services.AddScoped<RewardRedemptionsDataAccess>();
builder.Services.AddScoped<RewardRedemptionService>();
builder.Services.AddScoped<FamilyInvitationsDataAccess>();
builder.Services.AddScoped<FamilyInvitationService>();
builder.Services.AddScoped<ReportsDataAccess>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<RolesDataAccess>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<StatusesDataAccess>();
builder.Services.AddScoped<StatusService>();

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
