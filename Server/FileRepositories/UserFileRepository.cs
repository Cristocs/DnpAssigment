using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<User> AddAsync(User user)
    {
        List<User> users = await LoadUsersAsync();

        int maxId = users.Count > 0
            ? users.Max(u => u.Id)
            : 0;

        user.Id = maxId + 1;

        users.Add(user);

        await SaveUsersAsync(users);

        return user;
    }

    public async Task UpdateAsync(User user)
    {
        List<User> users = await LoadUsersAsync();

        int index = users.FindIndex(u => u.Id == user.Id);

        if (index == -1)
        {
            throw new KeyNotFoundException(
                $"User with ID {user.Id} was not found.");
        }

        users[index] = user;

        await SaveUsersAsync(users);
    }

    public async Task DeleteAsync(int id)
    {
        List<User> users = await LoadUsersAsync();

        User? user = users.FirstOrDefault(u => u.Id == id);

        if (user is null)
        {
            throw new KeyNotFoundException(
                $"User with ID {id} was not found.");
        }

        users.Remove(user);

        await SaveUsersAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        List<User> users = await LoadUsersAsync();

        User? user = users.FirstOrDefault(u => u.Id == id);

        if (user is null)
        {
            throw new KeyNotFoundException(
                $"User with ID {id} was not found.");
        }

        return user;
    }

    public IQueryable<User> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;

        List<User> users =
            JsonSerializer.Deserialize<List<User>>(json)
            ?? new List<User>();

        return users.AsQueryable();
    }

    private async Task<List<User>> LoadUsersAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<User>>(json)
               ?? new List<User>();
    }

    private async Task SaveUsersAsync(List<User> users)
    {
        string json = JsonSerializer.Serialize(users);

        await File.WriteAllTextAsync(filePath, json);
    }
}