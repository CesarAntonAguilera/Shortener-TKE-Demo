using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortener_TKE_Demo.Api.Tests
{
    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        //Override config or DI for tests here
    }
}
