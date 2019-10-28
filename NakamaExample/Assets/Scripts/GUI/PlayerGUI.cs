using Assets.Scripts.Manager;
using NakamaMinimalGame.PublicMatchState;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    public GameObject GUIFrame;
    public Text Me;
    public Text Enemy;
    public Slider MeHPSlider;
    public Slider MePowerSlider;
    public Slider EnemyHPSlider;
    public Slider EnemyPowerSlider;
    public Slider GCDSlider;
    public Slider CastBarSlider;

    public Button ButtonPrefab;

    [HideInInspector]
    public GameObject SelectedUnit;
    private PlayerController _selectedUnitPlayerController;
    public PlayerController MyPlayerController;
    private float _autoattackCD;
    private PlayerController _player;

    private bool _initiatedButtonBar;

    // Start is called before the first frame update
    void Start()
    {
        _player = this.gameObject.GetComponent<PlayerController>();
        GUIFrame.gameObject.SetActive(true);

        EnemyHPSlider.gameObject.SetActive(false);
        EnemyPowerSlider.gameObject.SetActive(false);
        GCDSlider.gameObject.SetActive(false);
        CastBarSlider.gameObject.SetActive(false);

        Me.text = MyPlayerController.CurrentHealth + "HP";
        MeHPSlider.value = MyPlayerController.CurrentHealth;
        MeHPSlider.maxValue = MyPlayerController.MaxHealth;
        MePowerSlider.value = MyPlayerController.CurrentPower;
        MePowerSlider.maxValue = MyPlayerController.MaxPower;
        Enemy.text = "";
    }

    // Update is called once per frame
    void OnGUI()
    {
        Me.text = MyPlayerController.CurrentHealth + " HP\n" + MyPlayerController.CurrentPower + " Mana";
        MeHPSlider.value = MyPlayerController.CurrentHealth;
        MeHPSlider.maxValue = MyPlayerController.MaxHealth;
        MePowerSlider.value = MyPlayerController.CurrentPower;
        MePowerSlider.maxValue = MyPlayerController.MaxPower;

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.transform.root.gameObject;
                if (go.layer == 8)
                {
                    SelectedUnit = go;
                    _selectedUnitPlayerController = go.GetComponent<PlayerController>();
                    EnemyHPSlider.maxValue = _selectedUnitPlayerController.MaxHealth;
                    EnemyPowerSlider.maxValue = _selectedUnitPlayerController.MaxPower;
                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.transform.root.gameObject;
                if (go.layer == 9)
                {
                    var targetPoint = hit.point;
                    var move = new Client_Message
                    {
                        Move = new Client_Message.Types.Client_Movement
                        {
                            AbsoluteCoordinates = true,
                            XAxis = targetPoint.x,
                            YAxis = targetPoint.y
                        }
                    };
                    PlayerManager.Instance.AddMessageToSend(move);
                }
            }
        }

        if (SelectedUnit != null && Input.GetKey(KeyCode.Escape))
        {
            SelectedUnit = null;
            _selectedUnitPlayerController = null;
        }

        if (SelectedUnit != null)
        {
            EnemyHPSlider.gameObject.SetActive(true);
            EnemyPowerSlider.gameObject.SetActive(true);
            EnemyHPSlider.value = _selectedUnitPlayerController.CurrentHealth;
            EnemyPowerSlider.value = _selectedUnitPlayerController.CurrentPower;
            Enemy.text = _selectedUnitPlayerController.CurrentHealth + " HP\n" + _selectedUnitPlayerController.CurrentPower + " Mana";
        }
        else
        {
            Enemy.text = "";
            EnemyHPSlider.gameObject.SetActive(false);
            EnemyPowerSlider.gameObject.SetActive(false);
        }

        //casting?
        if (_player.GCDUntil > Time.time)
        {
            GCDSlider.gameObject.SetActive(true);
            GCDSlider.value = _player.GCDUntil - Time.time;
        }
        else
        {
            GCDSlider.gameObject.SetActive(false);
        }

        if (_player.CastTimeUntil > Time.time)
        {
            CastBarSlider.gameObject.SetActive(true);
            CastBarSlider.value = _player.CastTimeUntil - Time.time;
        }
        else
        {
            CastBarSlider.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if(!_initiatedButtonBar && PlayerManager.Instance.Spawned)
        {
            _initiatedButtonBar = true;
            InitiateButtonBar();
        }
    }

    public void ButtonBarClick(GameDB_Lib.GameDB_Spell spell, Button button)
    {
        if (_player.GCDUntil < Time.time && _player.CastTimeUntil < Time.time)
        {
            var cast = new Client_Message
            {
                Cast = new Client_Message.Types.Client_Cast
                {
                    SpellId = spell.Id
                }
            };
            PlayerManager.Instance.AddMessageToSend(cast);
            if (!spell.IgnoresGCD)
            {
                GCDSlider.maxValue = spell.GlobalCooldown;
                _player.GCDUntil = Time.time + spell.GlobalCooldown;
            }
            if (spell.CastTime > 0)
            {
                CastBarSlider.maxValue = spell.CastTime;
                _player.CastTimeUntil = Time.time + spell.CastTime;
            }

            if (spell.Cooldown > 0)
            {
                button.GetComponent<Image>().color = Color.gray;

                System.Threading.Timer timer = null;
                timer = new System.Threading.Timer((obj) =>
                {
                    UnityThread.executeInUpdate(() => button.GetComponent<Image>().color = Color.white);
                    timer.Dispose();
                }, null, (int)(spell.Cooldown * 1000), System.Threading.Timeout.Infinite);
            }
        }
    }

    public void InitiateButtonBar()
    {
        int i = 0;
        foreach (var spell in GameManager.Instance.GameDB.Classes[PlayerManager.Instance.ClassName].Spells)
        {
            Button button = Instantiate(ButtonPrefab);
            button.transform.SetParent(GUIFrame.transform);
            button.transform.position = new Vector3(60 * i + 50, 50, 0);
            button.GetComponent<Button>().onClick.AddListener(() => ButtonBarClick(spell, button));
            button.transform.GetChild(0).GetComponent<Text>().text = spell.Name + "\nCD:" + spell.Cooldown + "\nCast:" + spell.CastTime;
            i++;
        }
    }
}
