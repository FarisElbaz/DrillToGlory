using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;


public class AuthenticationScript : MonoBehaviour
{


    [Header("Firebase Setup")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    [Header("Login Input Fields")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text errorText;

    [Header("Sign Up Input Fields")]
    public TMP_InputField emailInputSignUp;
    public TMP_InputField passwordInputSignUp;
    public TMP_InputField usernameInputSignUp;
    public TMP_Text errorTextSignUp;

    [Header("UI Panel")]
    public GameObject loginPanel;

    public GameObject SignUpPanel;

    private bool approvedSwitch = false;

    private void initilizeFB()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void LoginButton()
    {
        StartCoroutine(Login(emailInput.text, passwordInput.text));
    }

    public void SignUpButton()
    {
        StartCoroutine(SignUp(emailInputSignUp.text, passwordInputSignUp.text, usernameInputSignUp.text));
    }

    public void TogglePasswordVisibility()
    {
        if (passwordInput.contentType == TMP_InputField.ContentType.Password)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
        }
        passwordInput.ForceLabelUpdate();
    }

    private IEnumerator Login(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    errorText.text = "Please enter your email.";
                    break;
                case AuthError.MissingPassword:
                    errorText.text = "Please enter your password.";
                    break;
                case AuthError.WrongPassword:
                    errorText.text = "Incorrect password. Please try again.";
                    break;
                case AuthError.InvalidEmail:
                    errorText.text = "Invalid email format. Please check and try again.";
                    break;
                case AuthError.UserNotFound:
                    errorText.text = "No account found with this email. Please register first.";
                    break;
                default:
                    errorText.text = "Login failed. Please try again.";
                    break;
            }
        }
        else
        {
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);

            errorText.text = "Login successful! Preparing Drill bzzzz";
            errorText.color = Color.green;

            DG.Tweening.DOTween.KillAll();
            SceneManager.LoadScene(1);
            approvedSwitch = true;
        }
    }

    private IEnumerator SignUp(string _email, string _password, string _username)
    {
        string cleanEmail = _email.ToLower().Trim(); 
        var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(cleanEmail, _password);

        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

        if (RegisterTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    errorText.text = "Please enter your email.";
                    break;
                case AuthError.MissingPassword:
                    errorText.text = "Please enter your password.";
                    break;
                case AuthError.WeakPassword:
                    errorText.text = "Password is too weak. Please choose a stronger password.";
                    break;
                case AuthError.EmailAlreadyInUse:
                    errorText.text = "An account with this email already exists. Please log in instead.";
                    break;
                case AuthError.InvalidEmail:
                    errorText.text = "Invalid email format. Please check and try again.";
                    break;
                default:
                    errorText.text = "Registration failed. Please try again.";
                    break;
            }
        }
        else
        {
            User = RegisterTask.Result.User;

            if (User != null)
            {
                UserProfile profile = new UserProfile { DisplayName = _username };

                var ProfileTask = User.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                if (ProfileTask.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                    errorText.text = "Username update failed. Please try again.";
                }
                else
                {
                    Debug.LogFormat("User registered successfully: {0} ({1})", User.DisplayName, User.Email);
                    errorText.text = "Registration successful! You can now log in.";
                    errorText.color = Color.green;
                    toLoginButton();
                    approvedSwitch = true;
                }
            }
        }
    }

    public void EnterGame()
    {
        SceneManager.LoadScene(1);
    }

    public void toLoginButton() // This is from the SignUp panel
    {
        loginPanel.SetActive(true);
        SignUpPanel.SetActive(false);
    }

    public void toSignUpButton()
    {
        loginPanel.SetActive(false);
        SignUpPanel.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initilizeFB();
        loginPanel.SetActive(true);
        SignUpPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
