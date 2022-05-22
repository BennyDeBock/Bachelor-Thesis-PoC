using Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class DBInitializer
    {
        private readonly Context _context;

        public DBInitializer(Context context)
        {
            _context = context;
        }

        public async Task InitializeData()
        {
            _context.Database.EnsureDeleted();
            if (_context.Database.EnsureCreated())
            {
                var dataPlayers = ConvertCSVToData("C:\\Users\\BennyDB\\Documents\\School\\2021-2022\\Bachelorproef\\Bachelor-Thesis-PoC\\Datasets\\archive\\ittf_player_info.csv");

                AddCountries(_context, dataPlayers);

                foreach (DataRow r in dataPlayers.Rows)
                {
                    int id = int.Parse(r[0].ToString());
                    string name = r[1].ToString();
                    bool gender = r[3].ToString() == "Female" ? false : true;
                    bool active = r[5].ToString() == "Active" ? true : false;
                    string? playh = r[6].ToString() == "" ? null : r[6].ToString() == "-" ? null : r[6].ToString();
                    string? plays = r[7].ToString() == "" ? null : r[7].ToString() == "-" ? null : r[7].ToString();
                    string? grip = r[8].ToString() == "" ? null : r[8].ToString() == "-" ? null : r[8].ToString();

                    int? birthyear;
                    int value;
                    bool success = int.TryParse(r[4].ToString(), out value);
                    if (!success)
                    {
                        birthyear = null;
                    } else
                    {
                        birthyear = value;
                    }

                    Country country = _context.Countries.Single(c => c.Code == r[2].ToString());

                    _context.Players.Add(
                        new Player(id, name, gender, country, active, birthyear, playh, plays, grip)
                    );
                    
                }

                await _context.SaveChangesAsync();
            }
        }

        private static void AddCountries(Context _context, DataTable data)
        {
            HashSet<string> countrycodes = new();

            foreach (DataRow r in data.Rows)
            {
                if (!countrycodes.Contains(r[2].ToString()))
                {
                    countrycodes.Add(r[2].ToString());
                    var country = new Country(r[2].ToString());
                    _context.Countries.Add(
                        country
                    );
                }
            }
            
            _context.SaveChanges();
        }

        private static DataTable ConvertCSVToData(string filePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }
            return dt;
        }
    }
}
