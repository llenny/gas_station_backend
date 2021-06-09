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
using Microsoft.AspNetCore.Identity;

namespace gas_station.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly UserManager<User> _userManager;

        public OrdersController(DBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }


        public class order : Order
        {
            public string FuelName { get; set; }
            public string StationName { get; set; }
            public string UserName { get; set; }
            public int StationId { get; set; }
        }

        // получение всех заказов по пользователю
        // GET: api/Orders/5
        [HttpGet("{name}")]
        public async Task<ActionResult<List<order>>> GetOrder(string name)
        {
            List<order> data = new();

            var user = await _userManager.FindByNameAsync(name);
            using (_context)
            {
                // прописываем соединение с базой
                NpgsqlConnection conn = new("Server=127.0.0.1;Port=5432;Database=gas_station;User Id=postgres;Password=123;");
                conn.Open();
                // создаем транзакцию, чтобы все команды были в одной сессии
                using (NpgsqlTransaction tran = conn.BeginTransaction())
                {
                    //создаем новую команду для получения данных из базы (прописываем параметры и синтаксис)
                    NpgsqlCommand command = new NpgsqlCommand("call public.get_orders_of_user(@p_user_id, @p_cur)", conn);
                    command.CommandType = CommandType.Text;
                    // создаем именованные параметры (названия как в базе), прописываем их типы
                    command.Parameters.Add(new NpgsqlParameter("@p_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = user.Id });
                    NpgsqlParameter p = new NpgsqlParameter();
                    p.ParameterName = "@p_cur";
                    p.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Refcursor;
                    p.Direction = ParameterDirection.InputOutput;
                    p.Value = "p_cur";
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
                        order obj = new order();
                        obj.Id = Convert.ToInt32(dr.GetValue(0));
                        obj.UserId = Convert.ToInt32(dr.GetValue(1));
                        obj.FuelId = Convert.ToInt32(dr.GetValue(2));
                        obj.FuelName = Convert.ToString(dr.GetValue(3));
                        obj.Value = (float)dr.GetValue(4);
                        obj.Discount = (float)dr.GetValue(5);
                        obj.Price = (float)dr.GetValue(6);
                        obj.Sum = (float)dr.GetValue(7);
                        obj.StationId = Convert.ToInt32(dr.GetValue(8));
                        obj.StationName = Convert.ToString(dr.GetValue(9));
                        obj.CreateDate = Convert.ToDateTime(dr.GetValue(10));
                        
                        data.Add(obj);
                    }
                    dr.Close();

                    tran.Commit();
                    conn.Close();
                }

            }

            return data;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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


        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(order model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            // прописываем соединение с базой
            NpgsqlConnection conn = new("Server=127.0.0.1;Port=5432;Database=gas_station;User Id=postgres;Password=123;");
            conn.Open();

            //создаем новую команду для получения данных из базы (прописываем параметры и синтаксис)
            NpgsqlCommand command = new NpgsqlCommand("call public.create_order(@p_user_id, @p_fuel_id, @p_value, @p_price, @p_station_id)", conn);
            command.CommandType = CommandType.Text;
            // создаем именованные параметры (названия как в базе), прописываем их типы
            command.Parameters.Add(new Npgsql.NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = user.Id });
            command.Parameters.Add(new Npgsql.NpgsqlParameter("p_fuel_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = model.FuelId });
            command.Parameters.Add(new Npgsql.NpgsqlParameter("p_value", NpgsqlTypes.NpgsqlDbType.Real) { Value = model.Value });
            command.Parameters.Add(new Npgsql.NpgsqlParameter("p_price", NpgsqlTypes.NpgsqlDbType.Real) { Value = model.Price });
            command.Parameters.Add(new Npgsql.NpgsqlParameter("p_station_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = model.StationId });
            // выполняем созданную команду и получаем курсор с данными
            command.ExecuteNonQuery();

            conn.Close();

            return NoContent();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
