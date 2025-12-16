using RhyoliteOrbit.Models.TokenAuth;
using RhyoliteOrbit.Web.Controllers;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace RhyoliteOrbit.Web.Tests.Controllers;

public class HomeController_Tests : RhyoliteOrbitWebTestBase
{
    [Fact]
    public async Task Index_Test()
    {
        await AuthenticateAsync(null, new AuthenticateModel
        {
            UserNameOrEmailAddress = "admin",
            Password = "123qwe"
        });

        //Act
        var response = await GetResponseAsStringAsync(
            GetUrl<HomeController>(nameof(HomeController.StudentBillReport))
        );

        //Assert
        response.ShouldNotBeNullOrEmpty();
    }
}