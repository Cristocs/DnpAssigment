using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcUserRepository : IUserRepository
{
    private readonly AppContext ctx;

    public EfcUserRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }

    public async Task<User> AddAsync(User user)
    {
        EntityEntry<User> entry = await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();
        return entry.Entity; // ya trae el Id que ha generado la base de datos
    }

    public async Task UpdateAsync(User user)
    {
        if (!await ctx.Users.AnyAsync(x => x.Id == user.Id))
        {
            throw new KeyNotFoundException($"User with ID {user.Id} was not found.");
        }

        ctx.Users.Update(user);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        User? existing = await ctx.Users.SingleOrDefaultAsync(x => x.Id == id);
        if (existing is null)
        {
            throw new KeyNotFoundException($"User with ID {id} was not found.");
        }

        ctx.Users.Remove(existing);
        await ctx.SaveChangesAsync();
    }

    public async Task<User> GetSingleAsync(int id)
    {
        User? found = await ctx.Users.SingleOrDefaultAsync(x => x.Id == id);
        if (found is null)
        {
            throw new KeyNotFoundException($"User with ID {id} was not found.");
        }

        return found;
    }

    public IQueryable<User> GetMany()
    {
        return ctx.Users.AsQueryable();
    }
}
