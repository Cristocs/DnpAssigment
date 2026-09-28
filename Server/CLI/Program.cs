using FileRepositories;
using RepositoryContracts;
using CLI.UI;

ICommentRepository commentRepository = new CommentFileRepository();
IPostRepository postRepository = new PostFileRepository();
IUserRepository userRepository = new UserFileRepository();

CliApp app = new CliApp(
    userRepository,
    postRepository,
    commentRepository
);

await app.Start();