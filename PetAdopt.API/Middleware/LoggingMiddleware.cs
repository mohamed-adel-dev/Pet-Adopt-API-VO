using System.Diagnostics;

namespace PetAdopt.API.Middleware
{
    public class LoggingMiddleware
    {
        // Represents the next middleware component in the request processing pipeline.
        // Calling _next(context) passes the request to the next middleware.
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // httpContext represents all the information about the current HTTP request,response and User.
        public async Task InvokeAsync(HttpContext context)
        {
            // Start a stopwatch to measure the execution time of the request processing.
            var stopwatch = Stopwatch.StartNew();

            // Get the username from the HttpContext.
            // If the user is not authenticated, it defaults to "Anonymous".
            var user = context.User.Identity?.Name ?? "Anonymous";

            // Log the incoming request method and path
            Console.WriteLine($"Request Started: {context.Request.Method} {context.Request.Path}");
           
            // Call the next middleware in the pipeline
            await _next(context);

            // Stop the stopwatch after the next middleware has processed the request
            stopwatch.Stop();

            // After the next middleware has processed the request, log the response status code
            Console.WriteLine($"Response Status: {context.Response.StatusCode}");
                
            // Log the total execution time for processing the request
            Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
               
            // 
            Console.WriteLine($"User: {user}");

        }
    }
}
