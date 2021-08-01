using System;

namespace Service.Controllers.Utilities
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MainMenuElementAttribute : Attribute
    {
        public MainMenuElementAttribute(string elementText)
        {
            if (string.IsNullOrWhiteSpace(elementText))
            {
                throw new ArgumentException($"'{nameof(elementText)}' cannot be null or whitespace.", nameof(elementText));
            }

            ElementText = elementText;
        }

        public string ElementText { get; }
    }
}
