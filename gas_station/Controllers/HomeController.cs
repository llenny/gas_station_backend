using gas_station.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace gas_station.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _DBContext;


        public IEnumerable<Dictionary<string, object>> Serialize(Npgsql.NpgsqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }

        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols,
                                                Npgsql.NpgsqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }


        public HomeController(ILogger<HomeController> logger, DBContext DBContext)
        {
            _logger = logger;
            _DBContext = DBContext;
        }

        public IActionResult Index()
        {
            ViewData.Model = _DBContext.Stations.OrderBy(item=>item.Id).ToList();
            return View();
        }
        [HttpGet]
        public IActionResult GetStation(int id)
        {
            ViewData.Model = _DBContext.Stations.FirstOrDefault(i => i.Id == id);
            return View();
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee EmployeeModel)
        {
            var fromDB = _DBContext.Employees.Find(EmployeeModel.Id);
            fromDB.LastName = EmployeeModel.LastName;
            fromDB.FirstName = EmployeeModel.FirstName;
            fromDB.MiddleName = EmployeeModel.MiddleName;
            fromDB.Code = EmployeeModel.Code;
            _DBContext.SaveChanges();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
