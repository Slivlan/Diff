using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public class DiffRepository : IDiffRepository
    {
        private readonly DataContext context;

        public DiffRepository(DataContext context) 
        {
            this.context = context;
        }

        public Diff GetDiff(int id)
        {
            return context.Diffs.Where(p => p.Id == id).FirstOrDefault();    
        }

        public void PutLeft(int id, byte[] data)
        {
            var d = context.Diffs.Where(p => p.Id == id).FirstOrDefault();

            if (d == null)
            {
                d = new Diff(id);
                context.Diffs.Add(d);
            }

            d.Left = data;
            context.SaveChanges();
        }

        public void PutRight(int id, byte[] data)
        {
            var d = context.Diffs.Where(p => p.Id == id).FirstOrDefault();

            if(d == null)
            {
                d = new Diff(id);
                context.Diffs.Add(d);
            }

            d.Right = data;
            context.SaveChanges();
        }

    }
}
