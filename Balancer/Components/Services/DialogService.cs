namespace Balancer.Components.Services
{
    internal class DialogService
    {
        public Task<bool> ShowConfirmationDialog(
            string title,
            string message,
            string confirmButton = "Yes",
            string cancelButton = "No"
        )
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                return mainPage.DisplayAlert(title, message, confirmButton, cancelButton);
            }

            return Task.FromResult(false);
        }
    }
}
