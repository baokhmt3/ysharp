using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Notations.JSON;

namespace TestJSONParser
{
    class Program
    {
        // Note: fathers.json.txt was generated using:
        // http://experiments.mennovanslooten.nl/2010/mockjson/tryit.html
        const string FATHERS_TEST_FILE_PATH = @"..\..\fathers.json.txt";
        const string SMALL_TEST_FILE_PATH = @"..\..\small.json.txt";

        static Parser jsonParser = new Parser();

        static void Top10Youtube2013Test()
        {
            Console.WriteLine("Top 10 Youtube 2013 Test - JSON parse...");
            Console.WriteLine();
            System.Net.WebRequest www = System.Net.WebRequest.Create("https://gdata.youtube.com/feeds/api/videos?q=2013&max-results=10&v=2&alt=jsonc");
            using (System.IO.Stream stream = www.GetResponse().GetResponseStream())
            {
                Console.WriteLine("\tParsed by {0} in...", jsonParser.GetType().FullName);
                DateTime start = DateTime.Now;
                var parsed = jsonParser.Parse(stream);
                Console.WriteLine("\t\t{0} ms", (int)DateTime.Now.Subtract(start).TotalMilliseconds);
                Console.WriteLine();
                var data = parsed.JSONObject()["data"];
                var items = data.JSONObject()["items"].JSONArray();
                Console.WriteLine("Press a key...");
                Console.WriteLine();
                Console.ReadKey();
                foreach (object item in items)
                {
                    var post = item.JSONObject();
                    var title = (string)post["title"];
                    var category = (string)post["category"];
                    var uploaded = (string)post["uploaded"];
                    var player = post["player"].JSONObject();
                    var link = (string)player["default"];
                    Console.WriteLine("\t\"{0}\" (category: {1}, uploaded: {2})", title, category, uploaded);
                    Console.WriteLine("\t\tURL: {0}", link);
                    Console.WriteLine();
                }
                Console.WriteLine("Press a key...");
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            Top10Youtube2013Test();

            string small = System.IO.File.ReadAllText(SMALL_TEST_FILE_PATH);
            Console.WriteLine("Small Test - JSON parse... {0} bytes ({1} kb)", small.Length, ((decimal)small.Length / (decimal)1024));
            Console.WriteLine();

            Console.WriteLine("\tParsed by {0} in...", jsonParser.GetType().FullName);
            DateTime start = DateTime.Now;
            var obj = jsonParser.Parse(small);
            Console.WriteLine("\t\t{0} ms", (int)DateTime.Now.Subtract(start).TotalMilliseconds);
            Console.WriteLine();

            string json = System.IO.File.ReadAllText(FATHERS_TEST_FILE_PATH);
            Console.WriteLine("Fathers Test - JSON parse... {0} kb ({1} mb)", (int)(json.Length / 1024), (int)(json.Length / (1024 * 1024)));
            Console.WriteLine();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };
            Console.WriteLine("\tParsed by {0} in...", serializer.GetType().FullName);
            DateTime start1 = DateTime.Now;
            var msObj = serializer.DeserializeObject(json);
            Console.WriteLine("\t\t{0} ms", (int)DateTime.Now.Subtract(start1).TotalMilliseconds);
            Console.WriteLine();

            Console.WriteLine("\tParsed by {0} in...", jsonParser.GetType().FullName);
            DateTime start2 = DateTime.Now;
            var myObj = jsonParser.Parse(json);
            Console.WriteLine("\t\t{0} ms", (int)DateTime.Now.Subtract(start2).TotalMilliseconds);
            Console.WriteLine();

            Console.WriteLine("Press '1' to inspect our result object,\r\nany other key to inspect Microsoft's JS serializer result object...");
            var parsed = ((Console.ReadKey().KeyChar == '1') ? myObj : msObj);

            IList<object> items = (IList<object>)((IDictionary<string, object>)parsed)["fathers"];
            Console.WriteLine();
            Console.WriteLine("Found : {0} fathers", items.Count);
            Console.WriteLine();
            Console.WriteLine("Press a key to list them...");
            Console.WriteLine();
            Console.ReadKey();
            Console.WriteLine();
            foreach (object item in items)
            {
                var father = item.JSONObject();
                var name = (string)father["name"];
                var sons = father["sons"].JSONArray();
                var daughters = father["daughters"].JSONArray();
                Console.WriteLine("{0}", name);
                Console.WriteLine("\thas {0} son(s), and {1} daughter(s)", sons.Count, daughters.Count);
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("The end... Press a key...");

            Console.ReadKey();
        }
    }
}