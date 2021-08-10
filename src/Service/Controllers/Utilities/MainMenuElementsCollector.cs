using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Service.Controllers.Utilities
{
    public static class MainMenuElementsCollector
    {
        private static readonly object _lock = new object();

        private static IReadOnlyList<MainMenuElement> _elements;

        public static IReadOnlyList<MainMenuElement> GetMainMenuElements()
        {
            if(_elements == null)
            {
                lock(_lock)
                {
                    if(_elements == null)
                    {
                        var controllerTypes = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.IsAssignableTo(typeof(Controller)));

                        var actionsForMainMenu = controllerTypes
                            .SelectMany(t => t
                                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                .Where(m => m.GetCustomAttribute<MainMenuElementAttribute>() != null)
                                .Select(m => new { ControllerType = t, Action = m, Attribute = m.GetCustomAttribute<MainMenuElementAttribute>() }));

                        _elements = actionsForMainMenu
                            .OrderBy(a => a.Attribute.Order)
                            .Select(a => {
                                var controllerName = a.ControllerType.Name;
                                if(controllerName.EndsWith("Controller"))
                                {
                                    controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
                                }
                                return new MainMenuElement(
                                    controllerName,
                                    a.Action.Name,
                                    a.Attribute.ElementText);
                            })
                            .ToList();
                    }
                }
            }

            return _elements;
        }
    }
}
