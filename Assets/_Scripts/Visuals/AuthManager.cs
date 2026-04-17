using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;

    public static AuthManager Instance => instance;

    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser User { get; private set; }
    public string CurrentUserId => User?.UserId;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        Auth = FirebaseAuth.DefaultInstance;
        User = Auth.CurrentUser;
    }

    public IEnumerator Login(string email, string password, Action<FirebaseUser> onSuccess, Action<string> onError)
    {
        var loginTask = Auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning($"Login failed with {loginTask.Exception}");
            onError?.Invoke(ErrorsForLogin(loginTask.Exception.GetBaseException() as FirebaseException));
            yield break;
        }

        User = loginTask.Result.User;
        onSuccess?.Invoke(User);
    }

    public IEnumerator SignUp(string email, string password, string username, Action<FirebaseUser> onSuccess, Action<string> onError)
    {
        string cleanEmail = email.ToLower().Trim();
        var registerTask = Auth.CreateUserWithEmailAndPasswordAsync(cleanEmail, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogWarning($"Sign up failed with {registerTask.Exception}");
            onError?.Invoke(ErrorsForSignUp(registerTask.Exception.GetBaseException() as FirebaseException));
            yield break;
        }

        User = registerTask.Result.User;

        if (User == null)
        {
            onError?.Invoke("Registration failed. Please try again.");
            yield break;
        }

        UserProfile profile = new UserProfile { DisplayName = username };
        var profileTask = User.UpdateUserProfileAsync(profile);

        yield return new WaitUntil(() => profileTask.IsCompleted);

        if (profileTask.Exception != null)
        {
            Debug.LogWarning($"Failed to update profile with {profileTask.Exception}");
            onError?.Invoke("Username update failed. Please try again.");
            yield break;
        }

        onSuccess?.Invoke(User);
    }

    private static string ErrorsForLogin(FirebaseException firebaseEx)
    {
        if (firebaseEx == null)
        {
            return "Login failed. Please try again.";
        }

        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        return errorCode switch
        {
            AuthError.MissingEmail => "Please enter your email.",
            AuthError.MissingPassword => "Please enter your password.",
            AuthError.WrongPassword => "Incorrect password.",
            AuthError.InvalidEmail => "Invalid email format.",
            AuthError.UserNotFound => "No account found with this email. Please register first.",
            _ => "Login failed. Please try again."
        };
    }

    private static string ErrorsForSignUp(FirebaseException firebaseEx)
    {
        if (firebaseEx == null)
        {
            return "Registration failed. Please try again.";
        }

        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        return errorCode switch
        {
            AuthError.MissingEmail => "Please enter your email.",
            AuthError.MissingPassword => "Please enter your password.",
            AuthError.WeakPassword => "Password is too weak. Please choose a stronger password.",
            AuthError.EmailAlreadyInUse => "An account with this email already exists. Please log in instead.",
            AuthError.InvalidEmail => "Invalid email format. Please check and try again.",
            _ => "Registration failed. Please try again."
        };
    }
}
