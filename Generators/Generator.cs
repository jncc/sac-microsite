using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using JNCC.Microsite.SAC.Models.Data;
using JNCC.Microsite.SAC.Models.Website;
using JNCC.Microsite.SAC.Generators.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;

namespace JNCC.Microsite.SAC.Generators
{
    public static class Generator
    {

        public static void MakeSite()
        {
            var serviceScopeFactory = ServiceScopeFactory.GetServiceScopeFactory();

            ErrorPageGenerator.Generate(serviceScopeFactory);

            SitesGenerator.Generate(serviceScopeFactory);

            HabitatsGenerator.Generate(serviceScopeFactory);

            SpeciesGenerator.Generate(serviceScopeFactory);
        }
    }
}