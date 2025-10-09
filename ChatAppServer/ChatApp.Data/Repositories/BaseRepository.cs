using ChatApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ChatApp.Data.Repositories;

public abstract class BaseRepository<TEntity>(AppDbContext context, ILogger<BaseRepository<TEntity>> logger) : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context = context;
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    protected readonly ILogger<BaseRepository<TEntity>> _logger = logger;

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? filterBy = null, int? take = null, params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            IQueryable<TEntity> query = _dbSet;

            // filter som gör att vi kan hämta alla som är av en viss status (ex. Public)
            if (filterBy != null)
                query = query.Where(filterBy);

            // inludes inkluderar all olika tabeller som jag vill ha med (ex. .Include(x => x.User)
            if (includes != null && includes.Length != 0)
                foreach (var include in includes)
                    query = query.Include(include);

            // sortBy hanterar sorteringen av listan, ASC eller DESC och fält (ex. OrderBy(x => x.Created))
            if (sortBy != null)
                query = orderByDescending
                    ? query.OrderByDescending(sortBy)
                    : query.OrderBy(sortBy);

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching entities of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
         
    }
    public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Length != 0)
                foreach (var include in includes)
                    query = query.Include(include);

            var entity = await query.FirstOrDefaultAsync(findBy);
            return entity ?? null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching entity of type {EntityType}", typeof(TEntity).Name);
            throw;
        }

    }
    public virtual async Task<bool> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            return false;

        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(TEntity).Name);
            return false;
        }
    }
    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(expression);
        if (entity == default)
            return false;

        try
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return !await _dbSet.AnyAsync(expression);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity of type {EntityType}", typeof(TEntity).Name);
            return false;
        }
    }
    public virtual async Task<bool> AddAsync(TEntity entity)
    {
        if (entity == null)
            return false;

        try
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(TEntity).Name);
            return false;
        }
    }
    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbSet.AnyAsync(expression);
    }
}
