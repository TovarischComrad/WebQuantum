using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WebQuantum.Models;


namespace WebQuantum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Выбор оператора
        [HttpPost]
        public IActionResult Choose(string op)
        {
            BL.QLogic.Operator = op;
            return Ok();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Content(BL.QLogic.Operator);
        }

        // Добавление поля кубита в глобальную таблицу
        [HttpPost]
        public IActionResult AddQubit()
        {
            BL.QLogic.Table.Add(new List<string>());
            return Ok();
        }

        // Добавление оператора в поле кубита глобальной таблицы
        [HttpPost]
        public IActionResult AddOperator(int i)
        {
            BL.QLogic.Table[i].Add("I");
            return Ok();
        }

        // Изменение оператора в поле кубита глобальной таблицы
        [HttpPost]
        public IActionResult ChangeOperator(int i, int j, string op)
        {
            BL.QLogic.Table[i][j] = op;
            List<List<string>> test = BL.QLogic.Table;
            int x = 0;
            return Ok();    
        }

        // Удаление поля последнего кубита
        [HttpPost]
        public IActionResult DeleteQubit()
        {
            BL.QLogic.Table.RemoveAt(BL.QLogic.Table.Count - 1);
            return Ok();    
        }

        [HttpGet]
        public IActionResult GetBinary(int y, string temp)
        {
            int x = 0;
            for (int i = 0; i < BL.QLogic.Table.Count; i++)
            {
                if (BL.QLogic.Table[i][y].Contains(temp))
                {
                    x = i;
                }
            }
            return Content(x.ToString());
        }

        [HttpPost]
        public IActionResult TestConnector()
        {
            BL.QLogic.MakeCircuit();
            return Ok();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
