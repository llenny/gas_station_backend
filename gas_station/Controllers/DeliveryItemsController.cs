using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gas_station.Models;
using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace gas_station.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryItemsController : ControllerBase
    {
        private readonly DBContext _context;

        public DeliveryItemsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/DeliveryItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryItem>>> GetDeliveryItems()
        {
            return await _context.DeliveryItems.ToListAsync();
        }

        // создаем объект на основе DeliveryItem для передачи клиенту
        public class delivery_item : DeliveryItem
        {
            public string FuelName { get; set; }
        }

        // получение всех единиц выбранной заявки
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Object>>> GetDeliveryItem(int id)
        {
            List<object> data = new();
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
                    NpgsqlCommand command = new NpgsqlCommand("call public.get_delivery_items(@p_deliveryid, @p_cur)", conn);
                    command.CommandType = CommandType.Text;
                    // создаем именованные параметры (названия как в базе), прописываем их типы
                    command.Parameters.Add(new NpgsqlParameter("@p_deliveryid", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id });
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
                        delivery_item obj = new delivery_item();
                        obj.Id = Convert.ToInt32(dr.GetValue(0));
                        obj.FuelId = Convert.ToInt32(dr.GetValue(1));
                        obj.FuelName = Convert.ToString(dr.GetValue(2));
                        obj.DeliveryId = Convert.ToInt32(dr.GetValue(3));
                        obj.Value = (float)dr.GetValue(4);
                        obj.Price = (float)dr.GetValue(5);
                        obj.Sum = (float)dr.GetValue(6);
                        data.Add(obj);
                    }
                    dr.Close();

                    tran.Commit();
                    conn.Close();
                }

            }

            return data;
        }

        // PUT: api/DeliveryItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeliveryItem(int id, DeliveryItem deliveryItem)
        {
            if (id != deliveryItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(deliveryItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryItemExists(id))
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

        // POST: api/DeliveryItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DeliveryItem>> PostDeliveryItem(DeliveryItem deliveryItem)
        {
            _context.DeliveryItems.Add(deliveryItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeliveryItem", new { id = deliveryItem.Id }, deliveryItem);
        }

        private bool DeliveryItemExists(int id)
        {
            return _context.DeliveryItems.Any(e => e.Id == id);
        }
    }
}
