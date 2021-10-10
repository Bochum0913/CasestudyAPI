using CasestudyAPI.DAL.DomainClasses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CasestudyAPI.DAL.DAO
{
    public class BranchDAO
    {
        private AppDbContext _db;
        public BranchDAO(AppDbContext context)
        {
            _db = context;
        }
        //public async Task<bool> LoadBranchesFromFile(string path)
        //{
        //    bool addWorked = false;
        //    try
        //    {
        //        // clear out the old rows
        //        _db.Branches.RemoveRange(_db.Branches);
        //        await _db.SaveChangesAsync();
        //        var csv = new List<string[]>();
        //        var csvFile = path + "\\exercisesStoreRaw.txt";
        //        var lines = await System.IO.File.ReadAllLinesAsync(csvFile);
        //        foreach (string line in lines)
        //            csv.Add(line.Split(',')); // populate store with csv 
        //        foreach (string[] rawdata in csv)
        //        {
        //            Branch aStore = new Branch();
        //            aStore.Longitude = Convert.ToDouble(rawdata[0]);
        //            aStore.Latitude = Convert.ToDouble(rawdata[1]);
        //            aStore.Street = rawdata[2];
        //            aStore.City = rawdata[3];
        //            aStore.Region = rawdata[4];
        //            await _db.Branches.AddAsync(aStore);
        //            await _db.SaveChangesAsync();
        //        }
        //        addWorked = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return addWorked;
        //}
        public async Task<List<Branch>> GetThreeClosestBranches(float? lat, float? lon)
        {
            List<Branch> branchDetails = null;
            try
            {
                var latParam = new SqlParameter("@lat", lat);
                var lonParam = new SqlParameter("@lon", lon);
                var query = _db.Branches.FromSqlRaw("dbo.pGetThreeClosestBranches @lat, @lon", latParam,
                lonParam);
                branchDetails = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return branchDetails;
        }

    }
}
