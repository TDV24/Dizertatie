using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    //generate track
    public TMP_InputField corners;
    public TMP_InputField width;
    public TMP_InputField length;
    public GameObject errorText1;
    public GameObject errorText2;
    public TMP_Dropdown RemoveDropdown;
    //menu select modes
    List<String> tracks = new List<string>();
    public GameObject GenerateMenu;
    public GameObject RemoveMenu;
    public GameObject RemoveError;
    public GameObject StartMenu;
    public TMP_Dropdown RaceDropdown;
    public TMP_Dropdown RaceTrackDropdown;
    public String RaceTrack;
    public GameObject RaceTrackMenu;
    public GameObject RaceDriverMenu;
    public String GPTrack;
    public String GPDriver;
    public GameObject GPTrackMenu;
    public TMP_Dropdown GPTrackDropdown;
    public TMP_Dropdown GPDriverDropdown;
    public GameObject GPDriverMenu;
    public GameObject GPMenu;
    public GameObject GPQualyText;
    public String SeasonDriver;
    public TMP_Dropdown ChampionshipDriverDropdown;
    public GameObject ChampionshipDriverMenu;
    public GameObject ChampionshipMenu;
    public GameObject ChampionshipRaceMenu;
    public GameObject ChampionshipQualyText;
    public GameObject SelectPanel;
    public GameObject SelectMenu;
    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI NamesText;
    public TextMeshProUGUI PointsText;
    //upgrades
    public int Money;
    public TextMeshProUGUI MoneyAmount;
    public TextMeshProUGUI HandlingText;
    public TextMeshProUGUI TopSpeedText;
    public TextMeshProUGUI AccelerationText;
    //sound
    [SerializeField] Slider music;
    [SerializeField] Slider sfx;
    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            LoadMusicVolume();
        }
        else
        {
            LoadMusicVolume();
        }
        if(!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", 1);
            LoadSFXVolume();
        }
        else
        {
            LoadSFXVolume();
        }
        RaceTrackDropdown.ClearOptions();
        GPTrackDropdown.ClearOptions();
        RemoveDropdown.ClearOptions();
        if (!tracks.Contains("Track01")) 
            tracks.Add("Track01");
        if (!tracks.Contains("Track02")) 
            tracks.Add("Track02");
        if (!DataScript.createdtracks.Contains("Track01"))
            DataScript.createdtracks.Add("Track01");
        if (!DataScript.createdtracks.Contains("Track02"))
            DataScript.createdtracks.Add("Track02");
        RaceTrackDropdown.AddOptions(DataScript.createdtracks);
        GPTrackDropdown.AddOptions(DataScript.createdtracks);
        GetGeneratedTracks();
        if (PlayerPrefs.GetInt("GPQualy") == 1)
        {
            StartMenu.SetActive(false);
            GPMenu.SetActive(true);
            GPQualyText.SetActive(true);
        }
        if (PlayerPrefs.GetInt("SeasonQualy") == 1)
        {
            StartMenu.SetActive(false);
            ChampionshipMenu.SetActive(true);
            ChampionshipQualyText.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RoundText.text = "Current Round: Round " + (PlayerPrefs.GetInt("CurrentRound") + 1).ToString();
        if(PlayerPrefs.GetInt("CurrentRound") == tracks.Count)
            RoundText.text = "Championship ended";
        Money = PlayerPrefs.GetInt("Money");
        MoneyAmount.text = Money.ToString();
        switch(PlayerPrefs.GetInt("TopSpeedLevel"))
        {
            case 1:
                TopSpeedText.text = "1600";
                break;
            case 2:
                TopSpeedText.text = "2300";
                break;
            case 3:
                TopSpeedText.text = "MAX";
                break;
            default:
                TopSpeedText.text = "1100";
                break;
        }
        switch (PlayerPrefs.GetInt("HandlingLevel"))
        {
            case 1:
                HandlingText.text = "1200";
                break;
            case 2:
                HandlingText.text = "1600";
                break;
            case 3:
                HandlingText.text = "MAX";
                break;
            default:
                HandlingText.text = "800";
                break;
        }
        switch (PlayerPrefs.GetInt("AccelerationLevel"))
        {
            case 1:
                AccelerationText.text = "1300";
                break;
            case 2:
                AccelerationText.text = "1800";
                break;
            case 3:
                AccelerationText.text = "MAX";
                break;
            default:
                AccelerationText.text = "900";
                break;
        }

    }
    public void Quit()
    {
        PlayerPrefs.SetInt("SeasonQualy", 0);
        PlayerPrefs.SetInt("GPQualy", 0);
        Application.Quit();
    }
    public void StartRace()
    {
        PlayerPrefs.SetString("RaceDriver", RaceDropdown.options[RaceDropdown.value].text);
        PlayerPrefs.SetInt("GameMode", 1);
        if(RaceTrack == "Track01" || RaceTrack == "Track02")
            SceneManager.LoadScene(RaceTrack);
        else
        {
            PlayerPrefs.SetString("FileName", RaceTrack);
            SceneManager.LoadScene("LoadGenerated");
        }
    }
    public void SelectRaceTrack()
    {
        RaceTrack = RaceTrackDropdown.options[RaceTrackDropdown.value].text;
        RaceTrackMenu.SetActive(false);
        RaceDriverMenu.SetActive(true);
    }
    public void SelectGPTrack()
    {
        GPTrack = GPTrackDropdown.options[GPTrackDropdown.value].text;
        PlayerPrefs.SetString("GPTrack", GPTrack);
        GPTrackMenu.SetActive(false);
        GPDriverMenu.SetActive(true);
    }
    public void StartGPQualy()
    {
        if(PlayerPrefs.GetInt("GPQualy") == 0)
        {
            PlayerPrefs.SetInt("GameMode", 2);
            if (GPTrack == "Track01" || GPTrack == "Track02")
                SceneManager.LoadScene(GPTrack);
            else
            {
                PlayerPrefs.SetString("FileName", GPTrack);
                SceneManager.LoadScene("LoadGenerated");
            }
        }
    }
    public void StartSeasonQualy()
    {
        if(PlayerPrefs.GetInt("SeasonQualy") == 0)
        {
            PlayerPrefs.SetInt("GameMode", 4);
            if (tracks[PlayerPrefs.GetInt("CurrentRound")] == "Track01" || tracks[PlayerPrefs.GetInt("CurrentRound")] == "Track02")
                SceneManager.LoadScene(tracks[PlayerPrefs.GetInt("CurrentRound")]);
            else
            {
                PlayerPrefs.SetString("FileName", tracks[PlayerPrefs.GetInt("CurrentRound")]);
                SceneManager.LoadScene("LoadGenerated");
            }
        }
    }
    public void StartSeasonRace()
    {
        if (PlayerPrefs.GetInt("SeasonQualy") == 1)
        {
            PlayerPrefs.SetInt("GameMode", 5);
            if (tracks[PlayerPrefs.GetInt("CurrentRound")] == "Track01" || tracks[PlayerPrefs.GetInt("CurrentRound")] == "Track02")
                SceneManager.LoadScene(tracks[PlayerPrefs.GetInt("CurrentRound")]);
            else
            {
                PlayerPrefs.SetString("FileName", tracks[PlayerPrefs.GetInt("CurrentRound")]);
                SceneManager.LoadScene("LoadGenerated");
            }
        }
    }    
    public void ResetGPQuali()
    {
        GPQualyText.SetActive(false);
        PlayerPrefs.SetInt("GPQualy", 0);
    }
    public void SelectGPDriver()
    {
        GPDriver = GPDriverDropdown.options[GPDriverDropdown.value].text;
        PlayerPrefs.SetString("GPDriver", GPDriver);
        GPDriverMenu.SetActive(false);
        GPMenu.SetActive(true);
    }
    public void StartGPRace()
    {
        if(PlayerPrefs.GetInt("GPQualy") == 1)
        {
            PlayerPrefs.SetInt("GameMode", 3);
            if (GPTrack == "Track01" || GPTrack == "Track02")
                SceneManager.LoadScene(GPTrack);
            else
            {
                PlayerPrefs.SetString("FileName", GPTrack);
                SceneManager.LoadScene("LoadGenerated");
            }
        }
    }
    public void CheckExistingGame()
    {
        if (PlayerPrefs.GetInt("SavedGame") == 0)
        {
            SelectPanel.SetActive(false);
            SelectMenu.SetActive(false);
            ChampionshipDriverMenu.SetActive(true);
        }
        else
        {
            SelectPanel.SetActive(true);
        }
    }
    public void NewGame()
    {
        PlayerPrefs.SetInt("SavedGame", 0);
        PlayerPrefs.SetInt("SeasonQualy", 0);
        PlayerPrefs.SetString("SeasonDriver", "");
        PlayerPrefs.SetInt("Money", 0);
        PlayerPrefs.SetInt("TopSpeedLevel", 0);
        PlayerPrefs.SetInt("HandlingLevel", 0);
        PlayerPrefs.SetInt("AccelerationLevel", 0);
        PlayerPrefs.SetInt("CurrentRound", 0);
        PlayerPrefs.SetInt("Car#1Points", 0);
        PlayerPrefs.SetInt("Car#4Points", 0);
        PlayerPrefs.SetInt("Car#5Points", 0);
        PlayerPrefs.SetInt("Car#6Points", 0);
        PlayerPrefs.SetInt("Car#10Points", 0);
        PlayerPrefs.SetInt("Car#12Points", 0);
        PlayerPrefs.SetInt("Car#14Points", 0);
        PlayerPrefs.SetInt("Car#16Points", 0);
        PlayerPrefs.SetInt("Car#18Points", 0);
        PlayerPrefs.SetInt("Car#22Points", 0);
        PlayerPrefs.SetInt("Car#23Points", 0);
        PlayerPrefs.SetInt("Car#27Points", 0);
        PlayerPrefs.SetInt("Car#30Points", 0);
        PlayerPrefs.SetInt("Car#31Points", 0);
        PlayerPrefs.SetInt("Car#43Points", 0);
        PlayerPrefs.SetInt("Car#44Points", 0);
        PlayerPrefs.SetInt("Car#55Points", 0);
        PlayerPrefs.SetInt("Car#63Points", 0);
        PlayerPrefs.SetInt("Car#81Points", 0);
        PlayerPrefs.SetInt("Car#87Points", 0);
        SelectPanel.SetActive(false);
        SelectMenu.SetActive(false);
        ChampionshipDriverMenu.SetActive(true);
    }
    public void LoadGame()
    {
        SelectPanel.SetActive(false);
        SelectMenu.SetActive(false);
        ChampionshipMenu.SetActive(true);
    }
    public void SelectChampionshipDriver()
    {
        SeasonDriver = ChampionshipDriverDropdown.options[ChampionshipDriverDropdown.value].text;
        PlayerPrefs.SetInt("SavedGame", 1);
        PlayerPrefs.SetString("SeasonDriver", SeasonDriver);
        ChampionshipDriverMenu.SetActive(false);
        ChampionshipMenu.SetActive(true);
    }
    public void NextChampionshipRound()
    {
        if(PlayerPrefs.GetInt("CurrentRound") != tracks.Count)
        {
            ChampionshipMenu.SetActive(false);
            ChampionshipRaceMenu.SetActive(true);
        }
    }
    public void UpdateStandings()
    {
        NamesText.text = "";
        PointsText.text = "";
        Dictionary<String, int> order = new Dictionary<string, int>() 
        {
            {"Car#1", PlayerPrefs.GetInt("Car#1Points")},
            {"Car#4", PlayerPrefs.GetInt("Car#4Points")},
            {"Car#5", PlayerPrefs.GetInt("Car#5Points")},
            {"Car#6", PlayerPrefs.GetInt("Car#6Points")},
            {"Car#10", PlayerPrefs.GetInt("Car#10Points")},
            {"Car#12", PlayerPrefs.GetInt("Car#12Points")},
            {"Car#14", PlayerPrefs.GetInt("Car#14Points")},
            {"Car#16", PlayerPrefs.GetInt("Car#16Points")},
            {"Car#18", PlayerPrefs.GetInt("Car#18Points")},
            {"Car#22", PlayerPrefs.GetInt("Car#22Points")},
            {"Car#23", PlayerPrefs.GetInt("Car#23Points")},
            {"Car#27", PlayerPrefs.GetInt("Car#27Points")},
            {"Car#30", PlayerPrefs.GetInt("Car#30Points")},
            {"Car#31", PlayerPrefs.GetInt("Car#31Points")},
            {"Car#43", PlayerPrefs.GetInt("Car#43Points")},
            {"Car#44", PlayerPrefs.GetInt("Car#44Points")},
            {"Car#55", PlayerPrefs.GetInt("Car#55Points")},
            {"Car#63", PlayerPrefs.GetInt("Car#63Points")},
            {"Car#81", PlayerPrefs.GetInt("Car#81Points")},
            {"Car#87", PlayerPrefs.GetInt("Car#87Points")},
        };
        var ordered = order.OrderByDescending(kv => kv.Value).ToList();
        NamesText.text += "\n";
        PointsText.text += "Points\n";
        for (int i = 0; i < ordered.Count; i++)
        {
            if (ordered[i].Key == PlayerPrefs.GetString("SeasonDriver"))
            {
                NamesText.text += $"<color=orange>{i + 1}. {ordered[i].Key}</color>\n";
                PointsText.text += $"<color=orange>{ordered[i].Value}</color>\n";
            }
            else
            {
                NamesText.text += $"{i + 1}. {ordered[i].Key}\n";
                PointsText.text += $"{ordered[i].Value}\n";
            }    
        }
    }
    public void UpgradeCar(string upgradeType)
    {
        switch(upgradeType) 
        {
            case "topspeed":
                switch(PlayerPrefs.GetInt("TopSpeedLevel"))
                {
                    case 0:
                        if(Money >= 1100)
                        {
                            PlayerPrefs.SetInt("TopSpeedLevel", 1);
                            PlayerPrefs.SetInt("Money", Money - 1100); 
                        }
                        break;
                    case 1:
                        if(Money >= 1600)
                        {
                            PlayerPrefs.SetInt("TopSpeedLevel", 2);
                            PlayerPrefs.SetInt("Money", Money - 1600);
                        }
                        break;
                    case 2:
                        if(Money >= 2300)
                        {
                            PlayerPrefs.SetInt("TopSpeedLevel", 3);
                            PlayerPrefs.SetInt("Money", Money - 2300);                       
                        }
                        break;
                    default:
                        break;
                }
                break;
            case "handling":
                switch (PlayerPrefs.GetInt("HandlingLevel"))
                {
                    case 0:
                        if (Money >= 800)
                        {
                            PlayerPrefs.SetInt("HandlingLevel", 1);
                            PlayerPrefs.SetInt("Money", Money - 800);
                        }
                        break;
                    case 1:
                        if (Money >= 1200)
                        {
                            PlayerPrefs.SetInt("HandlingLevel", 2);
                            PlayerPrefs.SetInt("Money", Money - 1200);
                        }
                        break;
                    case 2:
                        if (Money >= 1600)
                        {
                            PlayerPrefs.SetInt("HandlingLevel", 3);
                            PlayerPrefs.SetInt("Money", Money - 1600);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case "acceleration":
                switch (PlayerPrefs.GetInt("AccelerationLevel"))
                {
                    case 0:
                        if (Money >= 900)
                        {
                            PlayerPrefs.SetInt("AccelerationLevel", 1);
                            PlayerPrefs.SetInt("Money", Money - 900);
                        }
                        break;
                    case 1:
                        if (Money >= 1300)
                        {
                            PlayerPrefs.SetInt("AccelerationLevel", 2);
                            PlayerPrefs.SetInt("Money", Money - 1300);
                        }
                        break;
                    case 2:
                        if (Money >= 1800)
                        {
                            PlayerPrefs.SetInt("AccelerationLevel", 3);
                            PlayerPrefs.SetInt("Money", Money - 1800);
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
    public void GenerateTrack()
    {
        if (int.Parse(corners.text) < 3)
        {
            errorText1.gameObject.SetActive(true);
            return;
        }
        if(int.Parse(width.text) < 300 || int.Parse(length.text) < 300) 
        { 
            errorText2.gameObject.SetActive(true);
            return;
        }
        PlayerPrefs.SetInt("TrackWidth", int.Parse(width.text));
        PlayerPrefs.SetInt("TrackLength", int.Parse(length.text));
        PlayerPrefs.SetInt("noOfCorners", int.Parse(corners.text));
        SceneManager.LoadScene("GeneratedTrack");
    }
    void GetGeneratedTracks()
    {
        RaceTrackDropdown.ClearOptions();
        GPTrackDropdown.ClearOptions();
        RemoveDropdown.ClearOptions();
        DataScript.generatedtracks.Clear();
        RaceTrackDropdown.AddOptions(DataScript.createdtracks);
        GPTrackDropdown.AddOptions(DataScript.createdtracks);
        string folderPath = Path.Combine(Application.persistentDataPath, "GeneratedTracks");
        if (Directory.Exists(folderPath))
        {
            string[] fullPaths = Directory.GetFiles(folderPath, "*.json");
            string[] fileNames = fullPaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();

            foreach (string name in fileNames)
            {
                DataScript.generatedtracks.Add(name);
                tracks.Add(name);
            }
            RaceTrackDropdown.AddOptions(DataScript.generatedtracks);
            GPTrackDropdown.AddOptions(DataScript.generatedtracks);
            RemoveDropdown.AddOptions(DataScript.generatedtracks);
        }
    }
    public void AccessRemoval()
    {
        if(DataScript.generatedtracks.Count > 0)
        {
            GenerateMenu.SetActive(false);
            RemoveMenu.SetActive(true);
        }
        else
            RemoveError.SetActive(true);
    }
    public void RemoveTrack()
    {
        string selectedTrackName = RemoveDropdown.options[RemoveDropdown.value].text;
        string filePath = Path.Combine(Application.persistentDataPath, "generatedtracks", selectedTrackName + ".json");
        File.Delete(filePath);
        DataScript.generatedtracks.Remove(selectedTrackName);
        tracks.Remove(selectedTrackName);
        GetGeneratedTracks();
        RaceTrackDropdown.RefreshShownValue();
        GPTrackDropdown.RefreshShownValue();
        RemoveDropdown.RefreshShownValue();
    }
    public void ChangeMusicVolume()
    {
        AudioListener.volume = music.value;
        SaveMusicVolume();
    }
    private void SaveMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", music.value);
    }
    private void LoadMusicVolume()
    {
        music.value = PlayerPrefs.GetFloat("MusicVolume");
    }
    public void ChangeSFXVolume()
    {
        SaveSFXVolume();
    }
    private void SaveSFXVolume()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfx.value);
    }
    private void LoadSFXVolume()
    {
        sfx.value = PlayerPrefs.GetFloat("SFXVolume");
    }
}
