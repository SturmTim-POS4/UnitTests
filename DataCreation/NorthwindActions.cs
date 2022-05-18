using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
using NorthwindDB;

namespace DataCreation;

public class NorthwindActions
{
    public Dictionary<string, List<int>> quantitysDictionary;

    public Dictionary<DateOnly, List<string>> dateProductDictionary;
    
    public Dictionary<string, Dictionary<string,int>> totalsPerEmployeeDictionary;

    public void QuantityListPerCustomerDictionary()
    {
        quantitysDictionary = new Dictionary<string, List<int>>();
        File.ReadAllLines("order_details_flat.csv").Skip(1).ToList().ForEach(x =>
        {
            if (!quantitysDictionary.ContainsKey(x.Split(";")[0]))
            {
                quantitysDictionary.Add(x.Split(";")[0],new List<int>());
            }
            quantitysDictionary[x.Split(";")[0]].Add(Int32.Parse(x.Split(";")[3]));
        });
    }

    public void ProductsPerDayDictionary()
    {
        dateProductDictionary = new Dictionary<DateOnly, List<string>>();
        File.ReadAllLines("order_details_flat.csv").Skip(1).ToList().ForEach(x =>
        {
            if (!dateProductDictionary.ContainsKey(DateOnly.Parse(x.Split(";")[1])))
            {
                dateProductDictionary.Add(DateOnly.Parse(x.Split(";")[1]),new List<string>());
            }
            dateProductDictionary[DateOnly.Parse(x.Split(";")[1])].Add(x.Split(";")[4]);
        });
        dateProductDictionary.Keys.ToList().ForEach( x => dateProductDictionary[x] = dateProductDictionary[x].Distinct().OrderBy(x => x).ToList());
    }

    public void TotalsPerProductAndEmployee()
    {
        totalsPerEmployeeDictionary = new Dictionary<string, Dictionary<string, int>>();
        File.ReadAllLines("order_details_flat.csv").Skip(1)
            .Select(x => x.Split(";"))
            .ToList()
            .ForEach(x =>
        {
            if (!totalsPerEmployeeDictionary.ContainsKey(x[2]))
            {
                totalsPerEmployeeDictionary.Add(x[2],new Dictionary<string, int>());
            }

            if (!totalsPerEmployeeDictionary[x[2]].ContainsKey(x[4]))
            {
                totalsPerEmployeeDictionary[x[2]].Add(x[4],0);
            }

            totalsPerEmployeeDictionary[x[2]][x[4]] += Int32.Parse(x[3]);
        });
    }

    public string fileTemporaryDb;
    public string fileTemporaryDblog;
    public NorthwindContext db;

    public void Dispose()
    {
        db.Database.CloseConnection();
    }
    public void InitData()
    {
        string sourceDb = @"C:\Users\timst\Downloads\NORTHWND.MDF";
        string exeDirectory = @"C:\Users\timst\Desktop\Schule\4. Klasse\POS1\UnitTestÜbung\DataCreation\";
        fileTemporaryDb = Path.Combine(exeDirectory, "TemporaryDbs", "NORTHWND.MDF");
        fileTemporaryDblog = Path.Combine(exeDirectory, "TemporaryDbs", "NORTHWND_log.ldf");
        if (File.Exists(fileTemporaryDb))
        {
            File.Delete(fileTemporaryDb);
            File.Delete(fileTemporaryDblog);
        }
        File.Copy(sourceDb,fileTemporaryDb);

        string connectionString =
            $@"server=(LocalDB)\mssqllocaldb;attachdbfilename={fileTemporaryDb};integrated security=True";
        var option = new DbContextOptionsBuilder<NorthwindContext>()
            .UseSqlServer(connectionString)
            .Options;
        db = new NorthwindContext(option);
        db.Database.OpenConnection();
    }

    public void InsertProducts()
    {
        File.ReadAllLines("products.csv")
            .Skip(1)
            .Select(x => x.Split(";"))
            .ToList()
            .ForEach(x =>
            {
                if (!db.Categories.Select(x => x.CategoryName).Contains(x[1]))
                {
                    db.Categories.Add(new Category() {CategoryName = x[1]});
                    db.SaveChanges();
                }
                if (!db.Suppliers.Select(x => x.CompanyName).Contains(x[4]))
                {
                    db.Suppliers.Add(new Supplier() {CompanyName = x[4]});
                    db.SaveChanges();
                }
                if (!db.Products.Select(x => x.ProductName).Contains(x[0]))
                {
                    db.Products.Add(new Product()
                    {
                        ProductName = x[0]
                    });
                    db.SaveChanges();
                }
            });
    }
}