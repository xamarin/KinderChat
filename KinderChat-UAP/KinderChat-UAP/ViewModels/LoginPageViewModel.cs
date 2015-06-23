using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinderChat.ServerClient;
using KinderChat_UAP.Common;
using KinderChat_UAP.Tools;

namespace KinderChat_UAP.ViewModels
{
    public class LoginPageViewModel : NotifierBase
    {
        private readonly IAuthenticationManager _authManager;

        public LoginPageViewModel(IAuthenticationManager authManager)
        {
            ClickSignupButtonCommand = new AsyncDelegateCommand(async o => { await ClickSignupButton(); },
                o => CanClickSignupButton);
            ClickTokenButtonCommand = new AsyncDelegateCommand(async o => { await ClickTokenButton(); },
                o => CanClickTokenButton);
            _authManager = authManager;
        }

        public LoginPageViewModel()
            : this(new AuthenticationManager())
        {
        }

        private string _emailAddress;
        private string _token;
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged();
            }
        }
        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                if (_emailAddress == value) return;
                _emailAddress = value;
                OnPropertyChanged();
                ClickSignupButtonCommand.RaiseCanExecuteChanged();
            }
        }

        public string Token
        {
            get { return _token; }
            set
            {
                if (_token == value) return;
                _token = value;
                OnPropertyChanged();
                ClickTokenButtonCommand.RaiseCanExecuteChanged();
            }
        }
        public AsyncDelegateCommand ClickSignupButtonCommand { get; private set; }
        public AsyncDelegateCommand ClickTokenButtonCommand { get; private set; }
        public event EventHandler<EventArgs> TokenSentSuccessful;
        public event EventHandler<EventArgs> TokenSentFailed;

        public event EventHandler<EventArgs> UserCreatedSuccessful;
        public event EventHandler<EventArgs> UserCreatedFailed;

        public bool CanClickSignupButton => !string.IsNullOrWhiteSpace(EmailAddress);

        public bool CanClickTokenButton => !string.IsNullOrWhiteSpace(Token);

        public async Task ClickTokenButton()
        {
            bool createResult;
            IsLoading = true;
            try
            {
                // TODO: Create Public/Private Key
                var result = await _authManager.CreateUserDeviceViaEmail(EmailAddress, Token, "test", "Test");
                if (result != null)
                {
                    var accessToken = await _authManager.Authenticate(result.UserDeviceId, result.AccessToken);
                    createResult = accessToken != null;
                }
                else
                {
                    createResult = false;
                }
            }
            catch (Exception ex) 
            {
                ChatDebugger.SendMessageDialogAsync(ex.Message, ex);
                createResult = false;
            }
            IsLoading = false;
            RaiseEvent(createResult ? UserCreatedSuccessful : UserCreatedFailed, EventArgs.Empty);
        }

        public async Task ClickSignupButton()
        {
            bool signupResult;
            IsLoading = true;
            try
            {
                signupResult = await _authManager.GetTokenEmail(EmailAddress);
            }
            catch (Exception ex )
            {
                ChatDebugger.SendMessageDialogAsync(ex.Message, ex);
                signupResult = false;
            }
            IsLoading = false;
            RaiseEvent(signupResult ? TokenSentSuccessful : TokenSentFailed, EventArgs.Empty);
        }
    }
}
