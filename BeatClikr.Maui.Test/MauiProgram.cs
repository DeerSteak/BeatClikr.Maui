using Microsoft.Extensions.Logging;
using Xunit.Runners.Maui;

namespace BeatClikr.Maui.Test;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp() => MauiApp
    .CreateBuilder()
    .ConfigureTests(new TestOptions
    {
      Assemblies =
      {
          typeof(MauiProgram).Assembly
      }
    })
    .UseVisualRunner()
    .Build();
}

