#load "./buildconfig.cake"

var config = BuildConfig.Create(Context, BuildSystem);
var verbosity = DotNetCoreVerbosity.Minimal;

Information($"SrcDir: '{config.SrcDir}'");
Information($"ArtifactsDir: '{config.ArtifactsDir}'");
Information($"SemVer: '{config.SemVer}'");
Information($"BuildVersion: '{config.BuildVersion}'");
Information($"BuildProfile: '{config.BuildProfile}'");

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("UnitTests");

Task("CI")
    .IsDependentOn("Default")
    .IsDependentOn("Docker-Compose-Up")
    .IsDependentOn("IntegrationTests")
    .IsDependentOn("Pack")
    .Finally(() => {
        Information("Running 'docker compose stop'...");
        StartProcess("docker-compose", "stop");
    });
/********************************************/
Task("Clean").Does(() => {
    EnsureDirectoryExists(config.ArtifactsDir);
    CleanDirectory(config.ArtifactsDir);
});

Task("Build").Does(() => {
    var settings = new DotNetCoreBuildSettings {
        Configuration = config.BuildProfile,
        NoIncremental = true,
        NoRestore = false,
        Verbosity = verbosity,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("TreatWarningsAsErrors", "true")
            .WithProperty("Version", config.SemVer)
            .WithProperty("AssemblyVersion", config.BuildVersion)
            .WithProperty("FileVersion", config.BuildVersion)
            .WithProperty("InformationalVersion", config.BuildVersion)
    };
    
    foreach(var sln in GetFiles($"{config.SrcDir}*.sln")) {
        DotNetCoreBuild(sln.FullPath, settings);
    }
});

Task("UnitTests").Does(() => {
    var settings = new DotNetCoreTestSettings {
        Configuration = config.BuildProfile,
        NoBuild = true,
        NoRestore = true,
        Logger = "trx",
        ResultsDirectory = config.TestResultsDir,
        Verbosity = verbosity
    };
    foreach(var testProj in GetFiles($"{config.SrcDir}tests/**/UnitTests.fsproj")) {
        DotNetCoreTest(testProj.FullPath, settings);
    }
});

Task("Docker-Compose-Up").Does(() => {
    StartProcess("docker-compose", "up -d");
    System.Threading.Tasks.Task.Delay(2000).Wait(); //I know... It will be replaced
});

Task("IntegrationTests").Does(() => {
    var settings = new DotNetCoreTestSettings {
        Configuration = config.BuildProfile,
        NoBuild = true,
        NoRestore = true,
        Logger = "trx",
        ResultsDirectory = config.TestResultsDir,
        Verbosity = verbosity
    };
    foreach(var testProj in GetFiles($"{config.SrcDir}tests/**/IntegrationTests.fsproj")) {
        DotNetCoreTest(testProj.FullPath, settings);
    }
});

Task("Pack").Does(() => {
    var settings = new DotNetCorePackSettings
    {
        Configuration = config.BuildProfile,
        OutputDirectory = config.ArtifactsDir,
        NoRestore = true,
        NoBuild = true,
        Verbosity = verbosity,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", config.SemVer)
            .WithProperty("AssemblyVersion", config.BuildVersion)
            .WithProperty("FileVersion", config.BuildVersion)
            .WithProperty("InformationalVersion", config.BuildVersion)
    };

    foreach(var proj in GetFiles($"{config.SrcDir}projects/**/*.csproj")) {
        DotNetCorePack(proj.FullPath, settings);
    }
});

RunTarget(config.Target);