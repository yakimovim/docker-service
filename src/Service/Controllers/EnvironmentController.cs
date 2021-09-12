using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Controllers
{
    public class EnvironmentController : Controller
    {
        [MainMenuElement("Environment")]
        [HttpGet]
        public IActionResult Index()
        {
            var environmentTypes = new[]
            {
                GetEnvironmentType("Machine", EnvironmentVariableTarget.Machine),
                GetEnvironmentType("User", EnvironmentVariableTarget.User),
                GetEnvironmentType("Process", EnvironmentVariableTarget.Process),
            };

            return View(environmentTypes);
        }

        private EnvironmentType GetEnvironmentType(string name, EnvironmentVariableTarget target)
        {
            var variables = Environment.GetEnvironmentVariables(target);

            return new EnvironmentType(
                name,
                variables.Keys.OfType<string>()
                .Select(key => new EnvironmentVariable(key, variables[key] as string))
                );
        }
    }

    public sealed class EnvironmentType
    {
        public EnvironmentType(string name, IEnumerable<EnvironmentVariable> variables)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Variables = variables ?? throw new ArgumentNullException(nameof(variables));
        }

        public string Name { get; }

        public IEnumerable<EnvironmentVariable> Variables { get; }
    }

    public sealed class EnvironmentVariable
    {
        public EnvironmentVariable(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; }

        public string Value { get; }
    }
}
