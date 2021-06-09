using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gas_station.Models;
using Npgsql;
using System.Data;

namespace gas_station.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly DBContext _context;

        public EmployeesController(DBContext context)
        {
            _context = context;
        }

        public class employee : Employee
        {
            public string Name { get; set; }
            public string PositionName { get; set; }
            public string StationName { get; set; }
            public int StationId { get; set; }

        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<List<employee>>> GetEmployees()
        {
            List<employee> data = new();
            using (_context)
            {
                // прописываем соединение с базой
                NpgsqlConnection conn = new("Server=127.0.0.1;Port=5432;Database=gas_station;User Id=postgres;Password=123;");
                conn.Open();
                // создаем транзакцию, чтобы все команды были в одной сессии
                //NpgsqlTransaction tran = conn.BeginTransaction();
                using (NpgsqlTransaction tran = conn.BeginTransaction())
                {
                    //создаем новую команду для получения данных из базы (прописываем параметры и синтаксис)
                    NpgsqlCommand command = new NpgsqlCommand("call public.get_employees(@p_cur)", conn);
                    command.CommandType = CommandType.Text;
                    // создаем именованные параметры (названия как в базе), прописываем их типы
                    NpgsqlParameter p = new()
                    {
                        ParameterName = "@p_cur",
                        NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Refcursor,
                        Direction = ParameterDirection.InputOutput,
                        Value = "p_cur"
                    };
                    command.Parameters.Add(p);
                    // выполняем созданную команду и получаем курсор с данными
                    command.ExecuteNonQuery();

                    // парсим курсор с данными, полученный на предыдущем этапе
                    command.CommandText = "fetch all in \"p_cur\"";
                    command.CommandType = CommandType.Text;

                    // для чтения данных создаем объект DataReader и сразу наполняем его данными
                    NpgsqlDataReader dr = command.ExecuteReader();

                    // заполняем список возвращаемых с сервера данных, идем по каждой записи курсора
                    while (dr.Read())
                    {
                        employee obj = new employee();

                        obj.Id = Convert.ToInt32(dr.GetValue(0));
                        obj.Code = Convert.ToInt32(dr.GetValue(1));
                        obj.PositionId = Convert.ToInt32(dr.GetValue(2));
                        obj.PositionName = Convert.ToString(dr.GetValue(3));
                        if (!(dr.GetValue(4) is DBNull))
                        {
                            obj.UserId = Convert.ToInt32(dr.GetValue(4));
                        };
                        obj.Name = Convert.ToString(dr.GetValue(5));
                        obj.Birthdate = Convert.ToDateTime(dr.GetValue(6));
                        obj.Status = Convert.ToBoolean(dr.GetValue(7));
                        obj.StationId = Convert.ToInt32(dr.GetValue(8));
                        obj.StationName = Convert.ToString(dr.GetValue(9));
                        data.Add(obj);
                    }
                    dr.Close();

                    tran.Commit();
                    conn.Close();
                }

            }

            return data;
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // создание нового сотрудника (без привязки в станции)
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(employee employee)
        {
            //_context.Employees.Add(employee);
            //await _context.SaveChangesAsync();
            Employee emp = new Employee
            {
                LastName = employee.LastName,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                PositionId = employee.PositionId,
                Birthdate = employee.Birthdate,
                Status = employee.Status,
                Code = employee.Code,
                CreateDate = new DateTime(),
                UpdateDate = new DateTime()
            };
            _context.Employees.Add(emp);
            foreach (Station station in employee.Stations) {
                emp.Stations.Add(station);
            }
            _context.SaveChanges();
            return NoContent();
            //return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
