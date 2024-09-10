var builder = DistributedApplication.CreateBuilder(args);

var rankMonkeyApi = builder.AddProject<Projects.RankMonkey_Server>("RestApi");

builder.AddProject<Projects.RankMonkey_Client>("WebApp")
    .WithReference(rankMonkeyApi);

var app = builder.Build();
app.Run();