using Assets.Scripts.Manager;
using NakamaMinimalGame.PublicMatchState;
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
        Me.text = MyPlayerController.CurrentHealth + "HP\n" + MyPlayerController.CurrentPower + "Mana";
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
            Enemy.text = _selectedUnitPlayerController.CurrentHealth + "HP\n" + _selectedUnitPlayerController.CurrentPower + "Mana";
        }
        else
        {
            Enemy.text = "";
            EnemyHPSlider.gameObject.SetActive(false);
            EnemyPowerSlider.gameObject.SetActive(false);
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

    public void ButtonBarClick(long spellId)
    {
        if (_player.GCDUntil < Time.time && _player.CastTimeUntil < Time.time)
        {
            var cast = new Client_Cast { SpellId = spellId };
            Assets.Scripts.Manager.PlayerManager.Instance.AddMessageToSend(cast);
            _player.GCDUntil = Time.time + 1.5f;
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
            button.GetComponent<Button>().onClick.AddListener(() => ButtonBarClick(spell.Id));
            button.transform.GetChild(0).GetComponent<Text>().text = spell.Name;
            i++;
        }
    }
}
