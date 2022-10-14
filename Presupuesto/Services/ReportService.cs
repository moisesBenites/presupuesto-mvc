using Presupuesto.Models;
using System.Threading;

namespace Presupuesto.Services
{
	public interface IReportService
	{
		Task<TransactionDetailReport> GetTransactionDetailReport(int userId, int month, int year, dynamic ViewBag);
		Task<TransactionDetailReport> GetTransactionDetailReportByAccount(int userId, int cuentaId, int month, int year, dynamic ViewBag);
		Task<IEnumerable<GetByWeeklyResponse>> GetWeeklyReport(int userId, int month, int year, dynamic ViewBag);
	}

	public class ReportService : IReportService
    {
		private readonly ITransactionRepository transactionRepository;
		private readonly HttpContext httpContext;

		public ReportService(ITransactionRepository transactionRepository, IHttpContextAccessor httpContextAccessor)
		{
			this.transactionRepository = transactionRepository;
			this.httpContext = httpContextAccessor.HttpContext;
		}

        public async Task<TransactionDetailReport> GetTransactionDetailReport(int userId, int month, int year, dynamic ViewBag)
		{
            (DateTime initialDate, DateTime finalDate) = GetInitialDate(month, year);

            var parameter = new TransactionByUserParameter()
            {
                finalDate = finalDate,
                initialDate = initialDate,
                userId = userId
            };

            var transactions = await transactionRepository.GetByUserId(parameter);
			TransactionDetailReport model = GenerateTransactionDetailReport(initialDate, finalDate, transactions);
			AssingViewBag(ViewBag, initialDate);

			return model;

		}

		public async Task<TransactionDetailReport> GetTransactionDetailReportByAccount(int userId, int cuentaId, int month, int year, dynamic ViewBag)
		{
			(DateTime initialDate, DateTime finalDate) = GetInitialDate(month, year);

			var request = new GetTransactionByAccount()
			{
				cuentaId = cuentaId,
				finalDate = finalDate,
				initialDate = initialDate,
				userId = userId
			};

			var transactions = await transactionRepository.GetByAccountId(request);

			TransactionDetailReport model = GenerateTransactionDetailReport(initialDate, finalDate, transactions);
			AssingViewBag(ViewBag, initialDate);

			return model;

		}

		public async Task<IEnumerable<GetByWeeklyResponse>> GetWeeklyReport(int userId, int month, int year, dynamic ViewBag)
		{
			(DateTime initialDate, DateTime finalDate) = GetInitialDate(month, year);
			var parameter = new TransactionByUserParameter()
			{
				finalDate = finalDate,
				initialDate = initialDate,
				userId = userId
			};

			AssingViewBag(ViewBag, initialDate);
			var model = await transactionRepository.GetByWeekly(parameter);

			return model;
		}

		// ******************************************************************
		// ************************* Metodos Privados ***********************
		// ******************************************************************
		#region Private
		private void AssingViewBag(dynamic ViewBag, DateTime initialDate)
		{
			ViewBag.lastMonth = initialDate.AddMonths(-1).Month;
			ViewBag.lastYear = initialDate.AddMonths(-1).Year;
			ViewBag.nextMonth = initialDate.AddMonths(1).Month;
			ViewBag.nextYear = initialDate.AddMonths(1).Year;
			ViewBag.url = httpContext.Request.Path + httpContext.Request.QueryString;
		}

		private static TransactionDetailReport GenerateTransactionDetailReport(DateTime initialDate, DateTime finalDate, IEnumerable<Transactions> transactions)
		{
			var model = new TransactionDetailReport();

			var transactionsByDate = transactions.OrderByDescending(x => x.date)
				.GroupBy(x => x.date)
				.Select(group => new TransactionDetailReport.TransactionByDate()
				{
					date = group.Key,
					transactions = group.AsEnumerable()
				});

			model.transactions = transactionsByDate;
			model.initialDate = initialDate;
			model.finalDate = finalDate;
			return model;
		}

		private (DateTime initialDate, DateTime finalDate) GetInitialDate(int month, int year)
		{
            DateTime initialDate;
            DateTime finalDate;

            if (month <= 0 || month > 12 || year <= 1900)
            {
                DateTime now = DateTime.Today;
                initialDate = new DateTime(now.Year, now.Month, 1);
            }
            else
            {
                initialDate = new DateTime(year, month, 1);
            }

            finalDate = initialDate.AddMonths(1).AddDays(-1);

            return (initialDate, finalDate);
        }

		#endregion
		// ******************************************************************
		// ************************* Metodos Privados ***********************
		// ******************************************************************
	}
}
