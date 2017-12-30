using Assets.InternalApis.Classes;
using Assets.InternalApis.Interfaces;
using Newtonsoft.Json;
using socket.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.InternalApis.Implementations
{
    public class UserController : IUserController
    {

        private Socket socketServer;
        public UserController()
        {
            player = new WulframPlayer();
            //player.Username = GetUsername();

            Debug.Log("UserController constructor:" + player.userName);
            SetupSocketConnection();
            var tmp = Loom.Current;
        }

        private WulframPlayer player;

        public event Action<WulframPlayer, string> LoginCompleted;
        public event Action<string> RegisterUserCompleted;

        public WulframPlayer GetWulframPlayerData()
        {
            if(string.IsNullOrEmpty(player.userName))
            {
                this.player.userName = GetUsername();
            }
            return player;
        }

        public void LoginUser(string username, string password)
        {
            socketServer.EmitJson("login", JsonConvert.SerializeObject(new { username = username, password  = password }));
        }

        public void RegisterUser(string username, string password, string email)
        {
            socketServer.EmitJson("registerNewUser", JsonConvert.SerializeObject(new { userName = username, password = password, email = email }));
        }

        public void RecordUnitKill(UnitType type)
        {
            this.RecordKill(type, 1);
            this.UpdatePlayer();
        }

        public void RecordUnitDeploy(UnitType type)
        {
            this.RecordDeploy(type, 1);
            this.UpdatePlayer();
        }

        public void RecordPlayerDeath(UnitType type)
        {
            this.RecordDeath(type);
            this.UpdatePlayer();
        }

        public void UpdateUserData()
        {
            this.UpdatePlayer();
        }

        private string GetUsername()
        {
            PlayerPrefs.DeleteAll();
            string defaultName = "";
            Debug.Log("defaultName:" + defaultName);

            var userString = this.player.userName;
            if (userString != "null")
            {
                // Auth'ed User
                switch (this.player.type)
                {
                    case "Moderator":
                        defaultName = "[MOD] " + userString;
                        break;

                    case "Developer":
                        defaultName = "[DEV] " + userString;
                        break;
                    default:
                        defaultName = userString;
                        break;
                }

                Debug.Log("defaultName:" + defaultName);
            }
            else
            {
                if (PlayerPrefs.HasKey("PlayerName"))
                {
                    defaultName = PlayerPrefs.GetString("PlayerName");
                    Debug.Log("defaultName:" + defaultName);
                }
                else
                {
                    defaultName = "GuestUser#" + new System.Random().Next(1, 9000);
                    Debug.Log("defaultName:" + defaultName);
                }
            }

            Debug.Log("defaultName:" + defaultName);
            PhotonNetwork.playerName = defaultName;
            return defaultName;
        }

        private void SetupSocketConnection()
        {
            //var serverUrl = "http://localhost:8080/";
            var serverUrl = "http://wulfram.com:1337/";
            socketServer = Socket.Connect(serverUrl);

            socketServer.On(SystemEvents.connect, () => {
                Debug.Log("Hello, Socket.io~");
            });

            socketServer.On("handshake", (string data) => {
                Debug.Log(data);
            });

            socketServer.On("loginCompleted", (string data) => {
                Loom.QueueOnMainThread(() =>
                {
                    Debug.Log("loginCompleted:" + data);
                    this.player = Newtonsoft.Json.JsonConvert.DeserializeObject<WulframPlayer>(data);
                    if (this.player.userName.ToLower() == "gotcha" || this.player.userName.ToLower() == "d4rksh4de" || this.player.userName.ToLower() == "knight1219")
                    {
                        this.player.type = "Developer";
                    }

                    GetUsername();
                    LoginCompleted.Invoke(player, "Login Complete");
                });
                
            });

            socketServer.On("loginFailed", (string data) => {
                Loom.QueueOnMainThread(() =>
                {
                    LoginCompleted.Invoke(null, "Login Failed");
                });
            });

            socketServer.On("registerComplete", (string data) => {
                Loom.QueueOnMainThread(() =>
                {
                    RegisterUserCompleted.Invoke(data);
                });
            });

            socketServer.On("registerFailed", (string data) => {
                Loom.QueueOnMainThread(() =>
                {
                    RegisterUserCompleted.Invoke(data);
                });
            });

            socketServer.On(SystemEvents.reconnect, (int reconnectAttempt) => {
                Debug.Log("Hello, Again! " + reconnectAttempt);
            });

            socketServer.On(SystemEvents.disconnect, () => {
                Debug.Log("Bye~");
            });
        }

        private void RecordKill(UnitType type, int value)
        {
            switch (type)
            {
                case UnitType.None:
                    break;
                case UnitType.Tank:
                    this.player.scores.tankKills += value;
                    break;
                case UnitType.Scout:
                    this.player.scores.scoutKills += value;
                    break;
                case UnitType.Cargo:
                    this.player.scores.cargoKills += value;
                    break;
                case UnitType.PowerCell:
                    this.player.scores.powercellKills += value;
                    break;
                case UnitType.RepairPad:
                    this.player.scores.repairpadKills += value;
                    break;
                case UnitType.RefuelPad:
                    this.player.scores.refullpadKills += value;
                    break;
                case UnitType.FlakTurret:
                    this.player.scores.flakturretKills += value;
                    break;
                case UnitType.GunTurret:
                    this.player.scores.gunturretKills += value;
                    break;
                case UnitType.MissleLauncher:
                    this.player.scores.misslelauncherKills += value;
                    break;
                case UnitType.Skypump:
                    this.player.scores.skypumpKills += value;
                    break;
                case UnitType.Darklight:
                    this.player.scores.darklightKills += value;
                    break;
                default:
                    break;
            }
        }

        private void RecordDeploy(UnitType type, int value)
        {
            switch (type)
            {
                case UnitType.PowerCell:
                    this.player.scores.powercellDeployed += value;
                    break;
                case UnitType.RepairPad:
                    this.player.scores.repairpadDeployed += value;
                    break;
                case UnitType.RefuelPad:
                    this.player.scores.refullpadDeployed += value;
                    break;
                case UnitType.FlakTurret:
                    this.player.scores.flakturretDeployed += value;
                    break;
                case UnitType.GunTurret:
                    this.player.scores.gunturretDeployed += value;
                    break;
                case UnitType.MissleLauncher:
                    this.player.scores.misslelauncherKills += value;
                    break;
                case UnitType.Skypump:
                    this.player.scores.skypumpDeployed += value;
                    break;
                case UnitType.Darklight:
                    this.player.scores.darklightDeployed += value;
                    break;
                default:
                    break;
            }
        }

        private void RecordDeath(UnitType type)
        {
            switch (type)
            {
                case UnitType.Tank:
                    this.player.scores.tankDeaths += 1;
                    break;
                case UnitType.Scout:
                    this.player.scores.scoutDeaths += 1;
                    break;
                default:
                    break;
            }
        }

        private void UpdatePlayer()
        {
            socketServer.EmitJson("updatePlayer", JsonConvert.SerializeObject(this.player));
        }
    }
}

public class Loom : MonoBehaviour
{
    public static int maxThreads = 8;
    static int numThreads;

    private static Loom _current;
    private int _count;
    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    static void Initialize()
    {
        if (!initialized)
        {

            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }

    }

    private List<Action> _actions = new List<Action>();
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }
    public static void QueueOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }

    public static Thread RunAsync(Action a)
    {
        Initialize();
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }

    }


    void OnDisable()
    {
        if (_current == this)
        {

            _current = null;
        }
    }



    // Use this for initialization
    void Start()
    {

    }

    List<Action> _currentActions = new List<Action>();

    // Update is called once per frame
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var a in _currentActions)
        {
            a();
        }
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }



    }
}