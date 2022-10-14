using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presupuesto.Models;
using Presupuesto.Services;
using System.Data;
using DataTable = System.Data.DataTable;

namespace Presupuesto.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IUserService userService;
        private readonly IRepositorioCuentas cuentasRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IMapper mapper;
        private readonly IReportService reportService;

        public TransactionController(IUserService userService, IRepositorioCuentas cuentasRepository,
            ICategoryRepository categoryRepository, ITransactionRepository transactionRepository, IMapper mapper, IReportService reportService)
        {
            this.userService = userService;
            this.cuentasRepository = cuentasRepository;
            this.categoryRepository = categoryRepository;
            this.transactionRepository = transactionRepository;
            this.mapper = mapper;
            this.reportService = reportService;
        }

        public async Task<IActionResult> Index(int month, int year)
        {
            var userId = userService.GetUserId();
            var model = await reportService.GetTransactionDetailReport(userId, month, year, ViewBag);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var userId = userService.GetUserId();
            var model = new CreateTransactionViewModel();
            model.Cuentas = await GetAccounts(userId);
            model.Categories = await GetCategories(userId, model.operationTypeId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionViewModel model)
        {
            var userId = userService.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Cuentas = await GetAccounts(userId);
                model.Categories = await GetCategories(userId, model.operationTypeId);
                return View(model);
            }

            var account = await cuentasRepository.GetById(model.cuentaId, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = await categoryRepository.GetById(model.categoryId, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            model.userId = userId;

            if (model.operationTypeId == OperationTypeEnum.Bill)
            {
                model.amount *= -1;
            }

            await transactionRepository.Create(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string url = null)
        {
            var userId = userService.GetUserId();
            var transaction = await transactionRepository.GetById(id, userId);

            if (transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = mapper.Map<UpdateTransactionViewModel>(transaction);

            if (model.operationTypeId == OperationTypeEnum.Bill)
            {
                model.montoAnterior = model.amount * -1;
            }

            model.cuentaAnteriorId = transaction.cuentaId;
            model.Categories = await GetCategories(userId, transaction.operationTypeId);
            model.Cuentas = await GetAccounts(userId);
            model.url = url;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateTransactionViewModel model)
        {
            var userId = userService.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Cuentas = await GetAccounts(userId);
                model.Categories = await GetCategories(userId, model.operationTypeId);
                return View(model);
            }

            var cuenta = await cuentasRepository.GetById(model.cuentaId, userId);

            if (cuenta is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = await categoryRepository.GetById(model.categoryId, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var transaction = mapper.Map<Transactions>(model);

            model.montoAnterior = model.amount;

            if (model.operationTypeId == OperationTypeEnum.Bill)
            {
                model.amount *= -1;
            }

            await transactionRepository.Update(transaction, model.montoAnterior, model.cuentaAnteriorId);

            if (string.IsNullOrEmpty(model.url))
            {
                return RedirectToAction("Index");

            }
            else
            {
                return LocalRedirect(model.url);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string url = null)
        {
            int userId = userService.GetUserId();
            var transactions = await transactionRepository.GetById(id, userId);

            if (transactions is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await transactionRepository.Delete(id);
            if (string.IsNullOrEmpty(url))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(url);
            }

        }

        public async Task<IEnumerable<SelectListItem>> GetAccounts(int userId)
        {
            var accounts = await cuentasRepository.Find(userId);
            return accounts.Select(x => new SelectListItem(x.nombre, x.id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> GetCategories([FromBody] OperationTypeEnum operationtype)
        {
            var userId = userService.GetUserId();
            var categories = await GetCategories(userId, operationtype);

            return Ok(categories);
        }

        public async Task<IActionResult> Weekly(int month, int year)
        {
            var userId = userService.GetUserId();
            IEnumerable<GetByWeeklyResponse> transactions = await reportService.GetWeeklyReport(userId, month, year, ViewBag);

            var groupedTransactions = transactions.GroupBy(x => x.week)
                .Select(x => new GetByWeeklyResponse()
                {
                    week = x.Key,
                    entry = x.Where(x => x.operationTypeId == OperationTypeEnum.Entry).Select(x => x.amount).FirstOrDefault(),
                    bill = x.Where(x => x.operationTypeId == OperationTypeEnum.Bill).Select(x => x.amount).FirstOrDefault(),
                }).ToList();

            if (year == 0 || month == 0)
            {
                var now = DateTime.Today;
                year = now.Year;
                month = now.Month;
            }

            var referenceDate = new DateTime(year, month, 1);
            var daysOfMonth = Enumerable.Range(1, referenceDate.AddMonths(1).AddDays(-1).Day);

            var segmentedDays = daysOfMonth.Chunk(7).ToList();

            for (int i = 0; i < segmentedDays.Count(); i++)
            {
                var week = i + 1;
                var initialDate = new DateTime(year, month, segmentedDays[i].First());
                var finalDate = new DateTime(year, month, segmentedDays[i].Last());
                var weekGroup = groupedTransactions.FirstOrDefault(x => x.week == week);

                if (weekGroup is null)
                {
                    groupedTransactions.Add(new GetByWeeklyResponse()
                    {
                        week = week,
                        initialDate = initialDate,
                        finalDate = finalDate
                    });
                }
                else
                {
                    weekGroup.initialDate = initialDate;
                    weekGroup.finalDate = finalDate;
                }
            }

            groupedTransactions = groupedTransactions.OrderByDescending(x => x.week).ToList();
            var model = new WeeklyReportViewModel();
            model.transactions = groupedTransactions;
            model.referenceDate = referenceDate;

            return View(model);
        }

        public async Task<IActionResult> Monthly(int year)
        {
            var userId = userService.GetUserId();

            if (year == 0)
            {
                year = DateTime.Today.Year;
            }

            var transactions = await transactionRepository.GetByMonthly(userId, year);
            var groupedTransactions = transactions.GroupBy(x => x.month)
                .Select(x => new GetByMonthResponse()
                {
                    month = x.Key,
                    bill = x.Where(x => x.operationTypeId == OperationTypeEnum.Bill).Select(x => x.amount).FirstOrDefault(),
                    entry = x.Where(x => x.operationTypeId == OperationTypeEnum.Entry).Select(x => x.amount).FirstOrDefault(),
                }).ToList();

            for (int month = 1; month <= 12; month++)
            {
                var transaction = groupedTransactions.FirstOrDefault(x => x.month == month);
                var referenceDate = new DateTime(year, month, 1);

                if (transaction is null)
                {
                    groupedTransactions.Add(new GetByMonthResponse()
                    {
                        month = month,
                        referenceDate = referenceDate,
                    });
                }
                else
                {
                    transaction.referenceDate = referenceDate;
                }
            }

            groupedTransactions = groupedTransactions.OrderByDescending(x => x.month).ToList();

            var model = new MontlyReportViewModel();
            model.transactions = groupedTransactions;
            model.year = year;

            return View(model);
        }

        public IActionResult ExcelReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExcelExportByMonth(int month, int year)
        {
            var initialDate = new DateTime(year, month, 1);
            var finalDate = initialDate.AddMonths(1).AddDays(-1);
            var userId = userService.GetUserId();

            var transactions = await transactionRepository.GetByUserId(new TransactionByUserParameter()
            {
                finalDate = finalDate,
                initialDate = initialDate,
                userId = userId
            });

            var fileName = $"Manejo Presupuesto - {initialDate.ToString("MMM yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExcelExportByYear(int year)
        {
            var initialDate = new DateTime(year, 1, 1);
            var finalDate = initialDate.AddYears(1).AddDays(-1);
            var userId = userService.GetUserId();

            var transactions = await transactionRepository.GetByUserId(new TransactionByUserParameter()
            {
                finalDate = finalDate,
                initialDate = initialDate,
                userId = userId
            });

            var fileName = $"Manejo Presupuesto - {initialDate.ToString("yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExcelExportAll()
        {
            var initialDate = DateTime.Today.AddYears(-100);
            var finalDate = DateTime.Today.AddYears(1000);
            var userId = userService.GetUserId();

            var transactions = await transactionRepository.GetByUserId(new TransactionByUserParameter()
            {
                finalDate = finalDate,
                initialDate = initialDate,
                userId = userId
            });

            var fileName = $"Manejo Presupuesto - {initialDate.ToString("dd-MM-yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<JsonResult> GetCalendarTransaction(DateTime start, DateTime end)
        {
            int userId = userService.GetUserId();
            var transactions = await transactionRepository.GetByUserId(new TransactionByUserParameter()
            {
                finalDate = end,
                initialDate = start,
                userId = userId
            });

            var calendarEvents = transactions.Select(t => new CalendarEvent()
            {
                Title = t.amount.ToString("N"),
                Start = t.date.ToString("yyyy-MM-dd"),
                End   = t.date.ToString("yyyy-MM-dd"),
                Color = (t.operationTypeId == OperationTypeEnum.Bill ? "Red" : null)
            });

            return Json(calendarEvents);
        }

        public async Task<JsonResult> GetTRansactionByDate(DateTime date)
        {
            int userId = userService.GetUserId();
            var transactions = await transactionRepository.GetByUserId(new TransactionByUserParameter()
            {
                finalDate = date,
                initialDate = date,
                userId = userId
            });

            return Json(transactions);
        }
        // **********************************************
        // ********** Funciones Privadas ****************
        // **********************************************
        private async Task<IEnumerable<SelectListItem>> GetCategories(int userId, OperationTypeEnum operationType)
        {
            var categories = await categoryRepository.GetCategories(userId, operationType);

            return categories.Select(x => new SelectListItem(x.name, x.id.ToString()));
        }

        private FileResult GenerateExcel(string fileName, IEnumerable<Transactions> transactions)
        {
            DataTable dataTable = new DataTable("Transacciones");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Fecha"),
                new DataColumn("Cuenta"),
                new DataColumn("Categoría"),
                new DataColumn("Nota"),
                new DataColumn("Monto"),
                new DataColumn("Ingreso/Gasto"),
            });

            foreach (var transaction in transactions)
            {
                dataTable.Rows.Add(transaction.date, transaction.cuenta, transaction.category, transaction.note, transaction.amount, transaction.operationTypeId == OperationTypeEnum.Bill ? "Ingreso" : "Gasto");
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}
