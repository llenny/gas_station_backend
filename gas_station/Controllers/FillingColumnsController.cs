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
    public class FillingColumnsController : ControllerBase
    {
        private readonly DBContext _context;

        public FillingColumnsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/FillingColumns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FillingColumn>>> GetFillingColumns()
        {
            return await _context.FillingColumns.ToListAsync();
        }

        // модель для возвращения единиц, доступных для заказа
        public class FuelToOrder
        {
            public string FuelName { get; set; }
            public int FuelId { get; set; }
            public double Price { get; set; }
            public DateTime BeginDate { get; set; }
            public DateTime EndDate { get; set; }
            public int FillingColumnId { get; set; }
            public double Value { get; set; }

        }

        // получение всех единиц топлива, которые можно заказать на выбранной колонке
        // GET: api/FillingColumns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<FuelToOrder>>> GetFillingColumn(int id)
        {
            List<FuelToOrder> data = new();
            // прописываем соединение с базой
            NpgsqlConnection conn = new("Server=127.0.0.1;Port=5432;Database=gas_station;User Id=postgres;Password=123;");
            conn.Open();
            // создаем транзакцию, чтобы все команды были в одной сессии
            NpgsqlTransaction tran = conn.BeginTransaction();

            //создаем новую команду для получения данных из базы (прописываем параметры и синтаксис)
            NpgsqlCommand command = new NpgsqlCommand("call public.get_fuels_to_order(@p_filling_column_id, @p_cur)", conn);
            command.CommandType = CommandType.Text;
            // создаем именованные параметры (названия как в базе), прописываем их типы
            command.Parameters.Add(new NpgsqlParameter("@p_filling_column_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id });
            NpgsqlParameter p = new NpgsqlParameter();
            p.ParameterName = "@p_cur";
            p.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Refcursor;
            p.Direction = ParameterDirection.InputOutput;
            p.Value = "p_cur";
            command.Parameters.Add(p);
            // выполняем созданную команду и получаем курсор с данными
            command.ExecuteNonQuery();

            // парсим курсор с данными, полученный на предыдущем этапе
            command.CommandText = "fetch all in p_cur";
            command.CommandType = CommandType.Text;

            // для чтения данных создаем объект DataReader и сразу наполняем его данными
            NpgsqlDataReader dr = command.ExecuteReader();
            // заполняем список возвращаемых с сервера данных, идем по каждой записи курсора
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    FuelToOrder obj = new();
                    obj.FuelId = Convert.ToInt32(dr.GetValue(0));
                    obj.FuelName = Convert.ToString(dr.GetValue(1));
                    obj.Price = (float)dr.GetValue(2);
                    obj.BeginDate = Convert.ToDateTime(dr.GetValue(3));
                    if (!(dr.GetValue(4) is DBNull))
                    {
                        obj.EndDate = Convert.ToDateTime(dr.GetValue(4));
                    }
                    obj.Value = Convert.ToDouble(dr.GetValue(5));
                    obj.FillingColumnId = Convert.ToInt32(dr.GetValue(6));
                    data.Add(obj);
                }
            }
            dr.Close();
            tran.Commit();
            conn.Close();

            return data;
        }

        // привязывание типов топлива к выбранной колонке
        [HttpPost]
        public async Task<IActionResult> PostFillingColumn(FillingColumn fillingColumn)
        {
            // прописываем соединение с базой
            NpgsqlConnection conn = new("Server=127.0.0.1;Port=5432;Database=gas_station;User Id=postgres;Password=123;");
            conn.Open();
            // создаем транзакцию, чтобы все команды были в одной сессии

            //создаем новую команду для получения данных из базы (прописываем параметры и синтаксис)
            NpgsqlCommand command = new NpgsqlCommand("call public.set_fuel_types_of_filling_column(@p_filling_column_id, @fuel_types_array)", conn);
            command.CommandType = CommandType.Text;
            // создаем именованные параметры (названия как в базе), прописываем их типы
            command.Parameters.Add(new NpgsqlParameter("@p_filling_column_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = fillingColumn.Id });
            NpgsqlParameter p = new NpgsqlParameter();
            p.ParameterName = "@fuel_types_array";
            p.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer;
            p.Direction = ParameterDirection.Input;
            List<int> fuelTypes = new();
            foreach (FuelType fuelType in fillingColumn.FuelTypes)
            {
                fuelTypes.Add(fuelType.Id);
            }
            int[] ft = fuelTypes.ToArray();
            fuelTypes.ToArray();
            p.Value = ft;
            command.Parameters.Add(p);
            // выполняем созданную команду и получаем курсор с данными
            command.ExecuteNonQuery();

            conn.Close();

            return NoContent();
        }

        private bool FillingColumnExists(int id)
        {
            return _context.FillingColumns.Any(e => e.Id == id);
        }
    }
}
