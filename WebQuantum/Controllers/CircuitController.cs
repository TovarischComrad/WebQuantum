using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebQuantum.Controllers
{
    public class CircuitController : Controller
    {
        public IActionResult Index()
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
        public IActionResult AddOperator(int i, int n)
        {
            for (int k = 0; k < n; k++)
            {
                BL.QLogic.Table[i].Add("I");
            }
            return Ok();
        }

        // Изменение оператора в поле кубита глобальной таблицы
        [HttpPost]
        public IActionResult ChangeOperator(int i, int j, string op)
        {
            List<List<string>> test = BL.QLogic.Table;
            BL.QLogic.Table[i][j] = op;

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
            int x = -1;
            List<List<string>> test = BL.QLogic.Table;
            for (int i = 0; i < BL.QLogic.Table.Count; i++)
            {
                if (BL.QLogic.Table[i][y].Contains(temp))
                {
                    x = i;
                }
            }
            return Content(x.ToString());
        }

        [HttpGet]
        public IActionResult TestConnector()
        {
            List<int> res = BL.QLogic.MakeCircuit();
            string result = string.Join(", ", res.ToArray());
            return Content(result);
        }
    }
}
