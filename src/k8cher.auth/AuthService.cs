using Dapr.Client;

namespace k8cher.auth
{
    public class AuthService
    {
        private readonly DaprClient _daprClient;
        private readonly UserManager<User> _userManager;

        public AuthService(DaprClient daprClient, UserManager<User> userManager)
        {
            _daprClient = daprClient;
            _userManager = userManager;
        }

        public async Task<ConfirmationResult> RegisterUser(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return ConfirmationResult.SendConfirmationLink;
            }

            // don't provide hints that user is regiserted, e-mail user account 
            if (result.Errors.Any(e => (e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName")))
            {
                // get another instance to check if the account has been confirmed
                var checkUser = await _userManager.FindByEmailAsync(user.Email);

                if (user.EmailConfirmed)
                {
                    return ConfirmationResult.SendAccountRequestMade;
                }
                else
                {
                    // increment access failed to prevent spamming unconfirmed account
                    await _userManager.AccessFailedAsync(user);
                    return ConfirmationResult.SendConfirmationLink;
                }
            }

            // this should hopefully not happen, bad experience if front end requirements different
            if (result.Errors.Any(e => (e.Code == "InvalidEmail")))
            {
                return ConfirmationResult.InvalidEmailFormat;
            }

            if (result.Errors.Any(e => e.Code.StartsWith("Password")))
            {
                Console.WriteLine($@"Error(s): UI did not properly regisering e-mail {user.Email}: 
                    string.Join(", ", result.Errors.Select(e => e.Description))}");

                return ConfirmationResult.InvalidPasswordFormat;
            }

            // for all other errors, don't provide insight into type of error in case of malicous acts
            return ConfirmationResult.ConfirmationFailed;
        }


        private async Task SendAccountExistsEmail(User user)
        {
            // todo - mbk: implement
            //send e-mail that tells users an account creation was submitted,
            // if not them, can ignore. If they want to reset password, provide link.
            throw new NotImplementedException();
        }

        public async Task SendConfirmAccountEmail(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var url = $"http://localhost:3000/auth/validate/{user.Id}/{token}";
            var body = $@"Click here to <a href=""{url}"">complete account registration!</a>";
            var subject = $"Confirm your k8cher account";

            await SendEmail(user.Email, body, subject);
        }

        private async Task SendEmail(string email, string body, string subject)
        {

            var metadata = new Dictionary<string, string>
            {
                ["emailFrom"] = "donotreply@domain.com", // todo - mbk: pull from global configuration
                ["emailTo"] = email,
                ["subject"] = subject
            };

            await _daprClient.InvokeBindingAsync("sendmail", "create", body, metadata);
        }

    }
}
