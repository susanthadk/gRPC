using Blog;
using Grpc.Core;

const string target = "127.0.0.1:50052";

Channel channel = new Channel("localhost", 50052, ChannelCredentials.Insecure);

await channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine("The client connected successfully");
});

var client = new BlogService.BlogServiceClient(channel);

//var newBlog = CreateBlog(client);
//ReadBlog(client);
//UpdateBlog(client, newBlog);
//DeleteBlog(client, newBlog);
await ListBlog(client);
channel.ShutdownAsync().Wait();
Console.ReadKey();

static Blog.Blog CreateBlog(BlogService.BlogServiceClient client)
{
    var response = client.CreateBlog(new CreateBlogRequest()
    {
        Blog = new Blog.Blog()
        {
            AuthorId = "Susantha",
            Title = "New blog!",
            Content = "Hello world, this is the new blog"
        }
    });
    Console.WriteLine("The blog " + response.Blog.Id + " was created !");

    return response.Blog;
}

static void ReadBlog(BlogService.BlogServiceClient client)
{
    try
    {
        var response = client.ReadBlog(new ReadBlogRequest()
        {
            BlogId = "66fa9e5685491b8f28fab6a2"
        });
        Console.WriteLine(response.Blog.ToString());
    }
    catch (RpcException e)
    {
        Console.WriteLine(e.Status.Detail);
    }
}

static void UpdateBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
{
    try
    {
        blog.AuthorId = "Updated Author";
        blog.Title = "Updated Title";
        blog.Content = "Updated Content";

        var response = client.UpdateBlog(new UpdateBlogRequest()
        {
            Blog = blog
        });

        Console.WriteLine(response.Blog.ToString());
    }
    catch (RpcException e)
    {
        Console.WriteLine(e.Status.StatusCode);
    }
}

static void DeleteBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
{
    try
    {
        var response = client.DeleteBlog(new DeleteBlogRequest() { BlogId = blog.Id });

        Console.WriteLine("The blog with id " + response.BlogId + " was deleted");
    }
    catch (RpcException e)
    {
        Console.WriteLine(e.Status.StatusCode);
    }
}

static async Task ListBlog(BlogService.BlogServiceClient client)
{
    try
    {
        var response = client.ListBlog(new ListBlogRequest() { });

        while (await response.ResponseStream.MoveNext())
        {
            await Console.Out.WriteLineAsync(response.ResponseStream.Current.Blog.ToString());
        }
        
    }
    catch (RpcException e)
    {
        Console.WriteLine(e.Status.Detail);
    }
}