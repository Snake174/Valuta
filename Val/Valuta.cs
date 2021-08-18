using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Val
{
    class Valuta
    {
        private string currentVal;
        private string currentValId;
        private IDataBase DB;
        public string Name { get; private set; }
        private static readonly Valuta instance = new Valuta();

        private Valuta()
        {
            Name = Guid.NewGuid().ToString();
            DB = new DataBaseFactory().GetDataBase();
        }

        public static Valuta Ptr()
        {
            return instance;
        }

        private string GetElementValue(XElement el, string name, string defValue = "")
        {
            string res = "";

            try
            {
                res = el.Element(name).Value;
            }
            catch (Exception e)
            {
                Console.WriteLine("Valuta.GetElementValue: " + e.Message);

                return defValue;
            }

            if (!String.IsNullOrEmpty(res))
                return res;

            return defValue;
        }

        public void GetCurrentVal()
        {
            INI ini = new INI("settings.ini");
            currentVal = ini.Read("current", "Valuta");

            currentValId = DB.GetValIdByName(currentVal);
        }

        /// <summary>
        /// Получение курса указанной валюты на текущую дату и занесение в базу данных
        /// </summary>
        public void GetValutaDaily()
        {
            Request r = new Request("http://cbr.ru/scripts/XML_daily.asp");
            r.ContentType = "text/xml";
            r.AddParam("date_req", DateTime.Now.ToShortDateString());

            try
            {
                string xml = r.Send();

                XDocument xdoc = XDocument.Parse(xml);
                IEnumerable<XElement> el = xdoc.Element("ValCurs").Elements("Valute");
                var valCurse = el.Where(x => x.Attribute("ID").Value == currentValId);

                ValCurse vc = new ValCurse
                {
                    Dt = DateTime.Now.ToString("yyyy-MM-dd"),
                    IdVal = currentVal,
                    Value = decimal.Parse(
                        valCurse.Select(x => GetElementValue(x, "Value"))
                            .FirstOrDefault()
                            .Replace(',', '.')
                    )
                };

                DB.SaveCurse(vc);
            }
            catch (Exception e)
            {
                Console.WriteLine("Valuta.GetValutaDaily: " + e.Message);
            }
        }

        /// <summary>
        /// Получение информации о всех валютах
        /// </summary>
        public void GetValInfo()
        {
            Request r = new Request("http://cbr.ru/scripts/XML_valFull.asp");
            r.ContentType = "text/xml";

            try
            {
                string xml = r.Send();

                XDocument xdoc = XDocument.Parse(xml);
                IEnumerable<XElement> el = xdoc.Element("Valuta").Elements("Item");

                foreach (XElement item in el)
                {
                    ValInfo vi = new ValInfo
                    {
                        IdCode = item.Attribute("ID").Value,
                        Name = GetElementValue(item, "Name"),
                        EngName = GetElementValue(item, "EngName"),
                        Nominal = Int32.Parse(GetElementValue(item, "Nominal", "1")),
                        ParentCode = GetElementValue(item, "ParentCode"),
                        ISONumCode = Int32.Parse(GetElementValue(item, "ISO_Num_Code", "0")),
                        ISOCharCode = GetElementValue(item, "ISO_Char_Code")
                    };

                    DB.SaveInfo(vi);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Valuta.GetValInfo: " + e.Message);
            }
        }

        /// <summary>
        /// Получение курса указанной валюты на заданную дату
        /// </summary>
        /// <param name="val">Наименование валюты</param>
        /// <param name="date">Дата (DD.MM.YYYY)</param>
        /// <returns></returns>
        public decimal GetValCurse(string val, string date)
        {
            // Сначала смотрим в базе
            try
            {
                decimal curse = DB.GetValCurse(val, DateTime.Parse(date).ToString("yyyy-MM-dd"));

                if (curse > 0)
                    return curse;
            }
            catch (Exception e)
            {
                Console.WriteLine("Valuta.GetValCurse: " + e.Message);

                return -1;
            }

            // Если нет в базе, берём с сайта
            string valId = DB.GetValIdByName(val);

            Request r = new Request("http://cbr.ru/scripts/XML_daily.asp");
            r.ContentType = "text/xml";
            r.AddParam("date_req", date);

            try
            {
                string xml = r.Send();

                XDocument xdoc = XDocument.Parse(xml);
                IEnumerable<XElement> el = xdoc.Element("ValCurs").Elements("Valute");
                var valCurse = el.Where(x => x.Attribute("ID").Value == valId);

                decimal res = decimal.Parse(
                    valCurse.Select(x => GetElementValue(x, "Value"))
                        .FirstOrDefault()
                        .Replace(',', '.')
                );

                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine("Valuta.GetValCurse: " + e.Message);

                return -1;
            }
        }
    }
}
