using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
public class AuthenticationScript : MonoBehaviour
{

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

    public void LoginButton()
    {
        StartCoroutine(AuthManager.Instance.Login(emailInput.text, passwordInput.text, OnLoginSuccess, OnLoginError));
    }

    public void SignUpButton()
    {
        StartCoroutine(AuthManager.Instance.SignUp(emailInputSignUp.text, passwordInputSignUp.text, usernameInputSignUp.text, OnSignUpSuccess, OnSignUpError));
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

    private void OnLoginSuccess(FirebaseUser user)
    {
        Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
        errorText.text = "Login successful! Preparing Drill bzzzz";
        errorText.color = Color.green;

        DG.Tweening.DOTween.KillAll();
        SceneManager.LoadScene(1);
    }

    private void OnLoginError(string message)
    {
        errorText.text = message;
    }

    private void OnSignUpSuccess(FirebaseUser user)
    {
        Debug.LogFormat("User registered successfully: {0} ({1})", user.DisplayName, user.Email);
        errorText.text = "Registration successful! You can now log in.";
        errorText.color = Color.green;
        toLoginButton();
    }

    private void OnSignUpError(string message)
    {
        errorText.text = message;
    }

    public FirebaseUser GetUserID()
    {
        return AuthManager.Instance.User;
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

    public void toSignUpButton() // This is from the Login panel
    {
        loginPanel.SetActive(false);
        SignUpPanel.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ = AuthManager.Instance;
        loginPanel.SetActive(true);
        SignUpPanel.SetActive(false);
    }
}
