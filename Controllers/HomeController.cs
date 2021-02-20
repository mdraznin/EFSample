using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EFSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace EFSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Tell .NET to inject the instance of the AdvDbContext object you created in the Startup class 
        //by adding a readonly field of the type AdvDbContext.
        private readonly AdvDbContext _db;

        public HomeController(ILogger<HomeController> logger, AdvDbContext dbContext)
        {
            //.NET looks at all parameters being passed into the HomeController 
            // and if they're in the collection of services, automatically fills in the parameters 
            // with an instance from the services collection object.
            _logger = logger;
            _db = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        /// <summary>
        /// Execute sp SalesLT.Product_GetAll
        /// </summary>
        /// <returns></returns>
        public IActionResult GetAll() 
        {
            List<Product> list;
            string sql = "EXEC SalesLT.Product_GetAll";
            list = _db.Products.FromSqlRaw<Product>(sql).ToList();
            Debugger.Break();
            return View("Index");
        }

        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <returns></returns>
        public IActionResult GetAProduct(int id = 706) 
        {
            List<Product> list;
            string sql = "EXEC SalesLT.Product_Get @ProductID";
            
            List<SqlParameter> parms = new List<SqlParameter> 
            {
                // Create parameter(s)    
                new SqlParameter { ParameterName = "@ProductID", Value = id }
            };
            
            list = _db.Products.FromSqlRaw<Product>(sql, parms.ToArray()).ToList();
            
            Debugger.Break();
            
            return View("Index");
        }

        public IActionResult CountAll() 
        {
            ScalarInt value;
            string sql = "EXEC SalesLT.Product_CountAll";
            value = _db.ScalarIntValue.FromSqlRaw<ScalarInt>(sql).AsEnumerable().FirstOrDefault();
            Debugger.Break();
            return View("Index");
        }

        public IActionResult UpdateListPrice() 
        {
            //Use the ExecuteSqlRaw() method on the Database object to call a data modification stored procedure.
            int rowsAffected;
            string sql = "EXEC SalesLT.Product_UpdateListPrice @ProductID, @ListPrice";
            
            List<SqlParameter> parms = new List<SqlParameter>
            { 
                // Create parameters    
                new SqlParameter { ParameterName = "@ProductID", Value = 706 },
                new SqlParameter { ParameterName = "@ListPrice", Value = 1500 }  
            };
            
            rowsAffected = _db.Database.ExecuteSqlRaw(sql, parms.ToArray());
            
            Debugger.Break();
        
            return View("Index");
        }

        public IActionResult MultipleResultSets() 
        {
            //When retrieving multiple result sets, you are almost writing pure ADO.NET code.  
            List<Product> black = new List<Product>();  
            List<Product> red = new List<Product>();  
            DbCommand cmd;  
            DbDataReader rdr;  
            
            string sql = "EXEC SalesLT.MultipleResultsColors";
            
            // Build command object  
            cmd = _db.Database.GetDbConnection().CreateCommand();  
            cmd.CommandText = sql;  
        
            // Open database connection  
            _db.Database.OpenConnection();  
            
            // Create a DataReader  
            rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);  
        
            // Build collection of Black products  
            while (rdr.Read()) 
            {    
                black.Add(new Product    
                {      
                    ProductID = rdr.GetInt32(0),      
                    Name = rdr.GetString(1),      
                    ProductNumber = rdr.GetString(2)    
                });  
            }
            
            // When you're done with the first result set, call the NextResult() method on the data reader object to advance to the next result set. Loop through all these records until the Read() method returns a false value. 
            // Advance to the next result set  
            rdr.NextResult();
            
            // Build collection of Red products  
            while (rdr.Read()) 
            {
                red.Add(new Product
                {
                    ProductID = rdr.GetInt32(0),
                    Name = rdr.GetString(1),
                    ProductNumber = rdr.GetString(2)
                });
            }
            
            Debugger.Break();
            
            // Close Reader and Database Connection  
            rdr.Close();    
            return View("Index");
        }


    }
}
