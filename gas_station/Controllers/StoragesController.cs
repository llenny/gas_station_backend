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
    public class StoragesController : ControllerBase
    {
        private readonly DBContext _context;

        public StoragesController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Storages
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Storage>>> GetStorage()
        //{
        //    return await _context.Storage.ToListAsync();
        //}

        // создаем объект на основе DeliveryItem для передачи клиенту
        public class storage : Storage
        {
            public string FuelName { get; set; }
            public string StationName { get; set; }

        }

        // получение всех единиц со склада определенной станции
        // GET: api/Storages/5
        //[HttpGet("{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<List<storage>>> GetStorage(int id)
        {
            List<storage> data = new();
            // прописываем соединение с базой
            NpgsqlConnection conn = new("Server=127.0.0.1;Port=5432;Database=gas_station;User Id=postgres;Password=123;");
            conn.Open();
            // создаем транзакцию, чтобы все команды были в одной сессии
            NpgsqlTransaction tran = conn.BeginTransaction();

            //создаем новую команду для получения данных из базы (прописываем параметры и синтаксис)
            NpgsqlCommand command = new NpgsqlCommand("call public.get_storage_of_station(@p_station_id, @p_cur)", conn);
            command.CommandType = CommandType.Text;
            // создаем именованные параметры (названия как в базе), прописываем их типы
            command.Parameters.Add(new NpgsqlParameter("@p_station_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id });
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
                    Console.WriteLine(dr[0]);
                    storage obj = new storage();
                    obj.Id = Convert.ToInt32(dr.GetValue(0));
                    obj.Value = (float)dr.GetValue(1);
                    obj.Status = Convert.ToBoolean(dr.GetValue(2));
                    obj.StationId = Convert.ToInt32(dr.GetValue(3));
                    obj.StationName = Convert.ToString(dr.GetValue(4));
                    obj.FuelId = Convert.ToInt32(dr.GetValue(5));
                    obj.FuelName = Convert.ToString(dr.GetValue(6));

                    data.Add(obj);
                }
            }
            dr.Close();
            tran.Commit();
            conn.Close();

            return data;
        }

        // PUT: api/Storages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStorage(int id, Storage storage)
        {
            if (id != storage.Id)
            {
                return BadRequest();
            }

            _context.Entry(storage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StorageExists(id))
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

        // POST: api/Storages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Storage>> PostStorage(Storage storage)
        //{
        //    _context.Storage.Add(storage);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetStorage", new { id = storage.Id }, storage);
        //}

        // DELETE: api/Storages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStorage(int id)
        {
            var storage = await _context.Storage.FindAsync(id);
            if (storage == null)
            {
                return NotFound();
            }

            _context.Storage.Remove(storage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StorageExists(int id)
        {
            return _context.Storage.Any(e => e.Id == id);
        }
    }
}
