using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public CliApp(
        IUserRepository userRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task Start()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("===== FORUM APPLICATION =====");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. Create post");
            Console.WriteLine("3. Add comment");
            Console.WriteLine("4. View all posts");
            Console.WriteLine("5. View post");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await CreateUser();
                    break;

                case "2":
                    await CreatePost();
                    break;

                case "3":
                    await AddComment();
                    break;

                case "4":
                    ViewAllPosts();
                    break;

                case "5":
                    await ViewPost();
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private async Task CreateUser()
    {
        Console.Write("Username: ");
        string username = Console.ReadLine();

        var existingUser = userRepository
            .GetMany()
            .FirstOrDefault(u => u.Username == username);

        if (existingUser != null)
        {
            Console.WriteLine("Username already taken.");
            return;
        }

        Console.Write("Password: ");
        string password = Console.ReadLine();

        User user = new User
        {
            Username = username,
            Password = password
        };

        User createdUser = await userRepository.AddAsync(user);

        Console.WriteLine($"User created with ID: {createdUser.Id}");
    }

    private async Task CreatePost()
    {
        Console.Write("Title: ");
        string title = Console.ReadLine();

        Console.Write("Body: ");
        string body = Console.ReadLine();

        Console.Write("User ID: ");
        int userId = int.Parse(Console.ReadLine());

        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        Post createdPost = await postRepository.AddAsync(post);

        Console.WriteLine($"Post created with ID: {createdPost.Id}");
    }

    private async Task AddComment()
    {
        Console.Write("Comment body: ");
        string body = Console.ReadLine();

        Console.Write("User ID: ");
        int userId = int.Parse(Console.ReadLine());

        var user = await userRepository.GetSingleAsync(userId);

        if (user == null)
        {
            Console.WriteLine("User does not exist.");
            return;
        }

        Console.Write("Post ID: ");
        int postId = int.Parse(Console.ReadLine());

        var post = await postRepository.GetSingleAsync(postId);

        if (post == null)
        {
            Console.WriteLine("Post does not exist.");
            return;
        }

        Comment comment = new Comment
        {
            Body = body,
            UserId = userId,
            PostId = postId
        };

        Comment createdComment = await commentRepository.AddAsync(comment);

        Console.WriteLine($"Comment created with ID: {createdComment.Id}");
    }

    private void ViewAllPosts()
    {
        var posts = postRepository.GetMany();

        Console.WriteLine();
        Console.WriteLine("===== POSTS =====");

        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }
    }

    private async Task ViewPost()
    {
        Console.Write("Post ID: ");
        int postId = int.Parse(Console.ReadLine());

        Post post = await postRepository.GetSingleAsync(postId);

        Console.WriteLine();
        Console.WriteLine($"===== {post.Title} =====");
        Console.WriteLine(post.Body);

        Console.WriteLine();
        Console.WriteLine("Comments:");

        var comments = commentRepository
            .GetMany()
            .Where(c => c.PostId == postId);

        foreach (Comment comment in comments)
        {
            Console.WriteLine($"- {comment.Body} (User ID: {comment.UserId})");
        }
    }
}