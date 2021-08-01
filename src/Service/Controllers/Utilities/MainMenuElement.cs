namespace Service.Controllers.Utilities
{
    public class MainMenuElement
    {
        public MainMenuElement(string controller, string action, string text)
        {
            if (string.IsNullOrWhiteSpace(controller))
            {
                throw new System.ArgumentException($"'{nameof(controller)}' cannot be null or whitespace.", nameof(controller));
            }

            if (string.IsNullOrWhiteSpace(action))
            {
                throw new System.ArgumentException($"'{nameof(action)}' cannot be null or whitespace.", nameof(action));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new System.ArgumentException($"'{nameof(text)}' cannot be null or whitespace.", nameof(text));
            }

            Controller = controller;
            Action = action;
            Text = text;
        }

        public string Controller { get; }
        public string Action { get; }
        public string Text { get; }
    }
}