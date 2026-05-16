using AutoMapper;
using TravelPlanner.Shared.DTOs.Expense;
using TravelPlanner.Shared.Models;

namespace FinanceService.Mappings
{
    public class FinanceMappingProfile : Profile
    {
        public FinanceMappingProfile()
        {
            CreateMap<Expense, ExpenseDto>();
            CreateMap<CreateExpenseDto, Expense>();
            CreateMap<UpdateExpenseDto, Expense>();
        }
    }
}