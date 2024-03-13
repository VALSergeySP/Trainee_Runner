using Firebase.Auth;
using Firebase.Database;
using JetBrains.Annotations;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Firebase Data")]
    public DatabaseReference DBreference;
    public FirebaseUser user;

    [Header("LeaderBoardFields")]
    [SerializeField] TMP_Text _top20Names;
    [SerializeField] TMP_Text _top20Scores;

    [SerializeField] TMP_Text _currentUserName;
    [SerializeField] TMP_Text _currentUserScore;

    private void Awake()
    {
        InitializeFirebase();
    }

    private void Start()
    {
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevelEvent += ResetLeaderBoard;
        StartCoroutine(UpdateUsernameDatabase(user.DisplayName));
    }

    private void InitializeFirebase()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;

        if(user == null)
        {
            Debug.Log("User Error!");
        }

        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

        Debug.Log("Setting up Firebase Database");
    }


    void ResetLeaderBoard()
    {
        StartCoroutine(DataUpdateRoutine());
    }

    IEnumerator DataUpdateRoutine()
    {
        int score = Singleton.Instance.ScorePointsManagerInstance.CurrentScore;

        yield return UpdateScoreDatabase(score);
        yield return LoadCurrentUserData();
        yield return LoadLeaderboardData();
    }


    IEnumerator UpdateUsernameDatabase(string username)
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("username").SetValueAsync(username);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
        } else
        {

        }
    }

    IEnumerator UpdateScoreDatabase(int score)
    {
        var DBGetTask = DBreference.Child("users").Child(user.UserId).Child("maxscore").GetValueAsync();

        yield return new WaitUntil(() => DBGetTask.IsCompleted);

        if (DBGetTask.Exception != null)
        {
            Debug.LogWarning($"Failed to load data from task with {DBGetTask.Exception}");
        } else if (DBGetTask.Result.Value != null)
        {
            int snapshotScore = int.Parse(DBGetTask.Result.Value.ToString());

            if (score < snapshotScore)
            {
                score = snapshotScore;
            }
        }


        var DBTask = DBreference.Child("users").Child(user.UserId).Child("maxscore").SetValueAsync(score);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log($"Score was setted to: {score}");
        }
    }

    IEnumerator LoadCurrentUserData()
    {
        var DBTask = DBreference.Child("users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to load data from task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            _currentUserScore.text = "0";
        } else
        {
            DataSnapshot snapshot = DBTask.Result;

            _currentUserScore.text = snapshot.Child("maxscore").Value.ToString();
        }

        _currentUserName.text = user.DisplayName;
    }

    IEnumerator LoadLeaderboardData()
    {
        var DBTask = DBreference.Child("users").OrderByChild("maxscore").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to load data from task with {DBTask.Exception}");
        } else
        {
            DataSnapshot snapshot = DBTask.Result;

            _top20Names.text = "";
            _top20Scores.text = "";

            int count = 0;

            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                count++;

                string username = childSnapshot.Child("username").Value.ToString();
                string score = childSnapshot.Child("maxscore").Value.ToString();

                _top20Names.text += $"{count}. {username}\n";
                _top20Scores.text += $"{score}\n";

                if(count >= 20)
                {
                    break;
                }
            }
        }
    }
}
