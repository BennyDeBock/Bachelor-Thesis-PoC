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

                var dataBlades = ConvertCSVToData("C:\\Users\\BennyDB\\Documents\\School\\2021-2022\\Bachelorproef\\Bachelor-Thesis-PoC\\Datasets\\TableTennisBladeslist.csv");

                AddBrands(_context, dataBlades);

                foreach (DataRow r in dataBlades.Rows)
                {
                    string name;
                    string brandName;

                    // Extract name and brand of the blade
                    var row = r[0].ToString();
                    Console.WriteLine(row);
                    if (row.StartsWith('('))
                    {
                        var split = row.Split(" ");
                        brandName = "No Brand";
                        name = row.Substring(split[0].Length + split[1].Length + 2);
                    }
                    else
                    {
                        var split = row.Split(" ");
                        brandName = split[0];
                        name = row.Substring(split[0].Length + 1);
                    }

                    double value;
                    int valueInt;
                    bool success = double.TryParse(r[1].ToString(), out value);
                    double? speed = success ? value : null;
                    success = double.TryParse(r[2].ToString(), out value);
                    double? control = success ? value : null;
                    success = double.TryParse(r[3].ToString(), out value);
                    double? stiffness = success ? value : null;
                    success = double.TryParse(r[4].ToString(), out value);
                    double? overall = success ? value : null;
                    var pricestr = r[5].ToString() == "" ? null : r[5].ToString().Substring(1);
                    success = double.TryParse(pricestr, out value);
                    double? price = success ? value : null;
                    success = int.TryParse(r[6].ToString(), out valueInt);
                    int ratings = success ? valueInt : 0;

                    Brand brand = _context.Brands.Single(c => c.Name == brandName);

                    _context.Blades.Add(
                        new Blade(name, brand, speed, control, stiffness, overall, price, ratings)
                    );

                }

                await _context.SaveChangesAsync();
            }
        }

        private static void AddBrands(Context _context, DataTable data)
        {
            HashSet<string> brands = new();

            foreach (DataRow r in data.Rows)
            {
                var row = r[0].ToString();
                var brandstr = "";

                if (row.StartsWith('('))
                {
                    brandstr = "No Brand";
                }
                else
                {
                    brandstr = row.Split(" ")[0];
                }

                if (!brands.Contains(brandstr))
                {
                    brands.Add(brandstr);
                    var brand = new Brand(brandstr);
                    _context.Brands.Add(brand);
                }
            }

            _context.SaveChanges();
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
