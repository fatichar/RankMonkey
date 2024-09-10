var builder = DistributedApplication.CreateBuilder(args);

var howRichApi = builder.AddProject<Projects.HowRich_Server>("RestApi");

builder.AddProject<Projects.HowRich_Client>("WebApp")
    .WithReference(howRichApi);

// ... other service registrations ...

var app = builder.Build();
app.Run();