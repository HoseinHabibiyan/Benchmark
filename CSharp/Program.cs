using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

class Program
{
    private static string connectionString = "Data Source =.; Initial Catalog = RouteBenchmark; Trusted_Connection=True;";
    private static List<Routes> _routes;
    private static List<City> _cities;

    static void Main(string[] args)
    {
        // load data from database
        _cities = GetCities();
        _routes = GetRoutes();

        // start 
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var result = Calculate(); 

        Report(result); 

        watch.Stop();
        // end

        // print duration
        var duration = watch.Elapsed.TotalMinutes;
        Console.WriteLine($"Duration : {duration}");

        Console.ReadKey();
    }

    static List<Result> Calculate()
    {
        var result = new List<Result>();

        int cityCount = _cities.Count;

        for (int i = 0; i < cityCount; i++)
        {
            for (int j = 0; j < cityCount; j++)
            {
                if (_cities[i].Id == _cities[j].Id)
                    continue;

                bool exist = Contains(_cities[i].Id, _cities[j].Id);

                if (!exist)
                {
                    result.Add(new Result()
                    {
                        SourceCityId = _cities[i].Id,
                        DestinationCityId = _cities[j].Id
                    });
                }
            }

            Console.WriteLine($"City : {i + 1}");
        }

        return result;
    }

    static bool Contains(int sourceCityId, int destinationCityId)
    {
        int count = _routes.Count();
        for (int i = 0; i < count; i++)
        {
            if (_routes[i].SourceCityId == sourceCityId && _routes[i].DestinationCityId == destinationCityId)
            {
                return true;
            }
        }
        return false;
    }

    static void Report(List<Result> results)
    {
        var stringbuilder = new StringBuilder();

        for (int i = 0; i < results.Count; i++)
        {
            stringbuilder.Append(results[i].SourceCityId).Append(" -> ").Append(results[i].DestinationCityId).Append('\n');
        }
        Console.WriteLine(stringbuilder.ToString());
        Console.WriteLine($"Result Count : {results.Count}");
    }



    static List<City> GetCities()
    {
        using IDbConnection db = new SqlConnection(connectionString);
        return db.Query<City>("select * from [dbo].Cities", commandType: CommandType.Text).ToList();
    }

    static List<Routes> GetRoutes()
    {
        using IDbConnection db = new SqlConnection(connectionString);
        return db.Query<Routes>("select * from [dbo].Routes", commandType: CommandType.Text).ToList();
    }

    public class City
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Routes
    {
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }
    }

    public class Result
    {
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }
    }

}