using InMemoryRepositories;
using RepositoryContracts;
using CLI.UI;

IUserRepository userRepository = new UserInMemoryRepositories();
IPostRepository postRepository = new PostInMemoryRepository();
ICommentRepository commentRepository = new CommentInMemoryRepository();

CliApp app = new CliApp(
    userRepository,
    postRepository,
    commentRepository
);

await app.Start();