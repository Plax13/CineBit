using Microsoft.EntityFrameworkCore;
using System;

namespace CineBit.Models;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly CinebitDbContext _context;
    protected readonly DbSet<T> _dbset;

    public Repository(CinebitDbContext cinebitDbContext)
    {
        _context = cinebitDbContext;
        _dbset = _context.Set<T>();
    }

    async Task<IEnumerable<T>> IRepository<T>.GetAllAsync()
    {
        return await _dbset.ToListAsync();
    }



    async Task<T> IRepository<T>.GetByIdAsync(int id)
    {
        return await _dbset.FindAsync(id);
    }
    async Task<T> IRepository<T>.AddAsync(T entity)
    {
        if(entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        await _dbset.AddAsync(entity);
        return entity;
    }

    void IRepository<T>.Update(T entity)
    {
        _dbset.Update(entity);
    }


    void IRepository<T>.Delete(T entity)
    {
        _dbset.Remove(entity);
    }

    async Task IRepository<T>.SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}