// using FluentEmail.Core;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Resend;
//
//
// namespace FitTech.Api.Tests.IntegrationTests;
// public partial class FluentTests
// {
//     private WebApplicationFactory<Program> _factory = null!;
//     private IResend _resend = null!;
//
//     public void Setup()
//     {
//         _factory = new WebApplicationFactory<Program>();
//
//         var http = _factory.CreateClient();
//
//         var opt = new ResendClientOptions()
//         {
//             ApiUrl = http.BaseAddress!.ToString(),
//         };
//
//         _resend = ResendClient.Create(opt, http);
//
//         Email.DefaultSender = new ResendSender(_resend);
//     }
//
//     [Test]
//     public async Task EmailSend()
//     {
//         var resp = await Email
//             .From("from@example.com")
//             .To("to@example.com")
//             .Subject("Unit testing from Fluent")
//             .Body("Html body", true)
//             .PlaintextAlternativeBody("Text body")
//             .SendAsync();
//
//         await Assert.That(resp).NotBeNull();
//         await Assert.That(resp.Successful).BeTrue();
//         await Assert.That(resp.MessageId).NotBeEmpty();
//     }
// }
