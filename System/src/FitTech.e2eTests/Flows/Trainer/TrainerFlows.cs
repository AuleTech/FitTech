using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Playwright;
using TUnit.Playwright;

namespace FitTech.e2eTests.Flows.Trainer;

[Property("Type", "e2e")]
public class TrainerFlows : PageTest
{
    [Test]
    public async Task Login()
    {
        await Page.GotoAsync("http://localhost:5174");
        await Expect(Page).ToHaveURLAsync(new Regex(".*login"));

        await Page.GetByTestId("email").FillAsync("admin@fittech.es");
        await Page.GetByTestId("password").FillAsync("FitTech2025!");
        await Page.GetByText("LOGIN").ClickAsync();
        await Expect(Page.GetByTestId("title")).ToHaveTextAsync("FitTech");
    }
}
