using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using FinanceService.Data;
using FinanceService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TravelPlanner.Shared.DTOs.Expense;
using TravelPlanner.Shared.Interfaces;

namespace FinanceService
{
    internal sealed class FinanceService : StatelessService, IExpenseServices
    {
        private readonly IExpenseServices _expenseServices;

        public FinanceService(StatelessServiceContext context)
            : base(context)
        {
            var configPackage = context.CodePackageActivationContext
                .GetConfigurationPackageObject("Config");

            var connectionString = configPackage.Settings
                .Sections["ConnectionStrings"]
                .Parameters["DefaultConnection"].Value;

            var services = new ServiceCollection();
            services.AddDbContext<FinanceDbContext>(options =>
                options.UseSqlServer(connectionString));


            services.AddAutoMapper(cfg => cfg.AddProfile<Mappings.FinanceMappingProfile>());
            services.AddScoped<IExpenseServices, ExpenseServices>();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var provider = services.BuildServiceProvider();
            _expenseServices = provider.GetRequiredService<IExpenseServices>();
        }

        public Task<IEnumerable<ExpenseDto>> GetAllByTripAsync(int tripId)
            => _expenseServices.GetAllByTripAsync(tripId);
        public Task<ExpenseDto> GetByIdAsync(int id)
            => _expenseServices.GetByIdAsync(id);
        public Task<ExpenseDto> CreateAsync(CreateExpenseDto dto)
            => _expenseServices.CreateAsync(dto);
        public Task<ExpenseDto> UpdateAsync(int id, UpdateExpenseDto dto)
            => _expenseServices.UpdateAsync(id, dto);
        public Task<bool> DeleteAsync(int id)
            => _expenseServices.DeleteAsync(id);
        public Task<decimal> GetTotalExpensesAsync(int tripId)
            => _expenseServices.GetTotalExpensesAsync(tripId);
        
        public Task<BudgetSummaryDto> GetBudgetSummaryAsync(int tripId, decimal plannedBudget)
            => _expenseServices.GetBudgetSummaryAsync(tripId, plannedBudget);

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            => this.CreateServiceRemotingInstanceListeners();
    }
}