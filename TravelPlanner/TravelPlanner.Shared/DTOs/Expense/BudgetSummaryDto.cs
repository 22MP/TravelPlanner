using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelPlanner.Shared.DTOs.Expense
{
    public class BudgetSummaryDto
    {
        public decimal TotalExpenses { get; set; }
        public decimal PlannedBudget { get; set; }
        public decimal RemainingBudget { get; set; }
    }
}
