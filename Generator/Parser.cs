using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class Parser
    {
        public List<string> Parse(string path)
        {
            List<string> list = new List<string>();
            try
            {
                using (var sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        list.Add(sr.ReadLine());
                    }
                }

                return list;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public List<(string, List<string>)> ParseCSV(string path)
        {
            var list = new List<(string, List<string>)>();

            try
            {
                using(var sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        var row = sr.ReadLine();
                        var columns = row.Split(';').ToList();
                        list.Add((columns[0], columns.Skip(1).ToList()));
                    }
                }

                return list;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public List<(string,string,int,string)> ParseCSVMember(string path)
        {
            var list = new List<(string, string, int, string)>();

            try
            {
                using (var sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        var row = sr.ReadLine();
                        var columns = row.Split(';').ToList();
                        if (string.IsNullOrWhiteSpace(columns[1]))
                            columns[1] = "null";
                        if (columns[2] == "-")
                            columns[2] = new Random().Next(20, 70).ToString();
                        if (columns[3] == null)

                        if(list.Count == 47)
                            Console.WriteLine();
                        list.Add((columns[0], columns[1], int.Parse(columns[2]) , columns[3]));
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
