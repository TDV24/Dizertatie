using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
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
    //menu select modes
    public String GPTrack;
    public String GPDriver;
    public GameObject GPTrackMenu;
    public TMP_Dropdown GPDriverDropdown;
    public GameObject GPDriverMenu;
    public GameObject GPMenu;
    public String SeasonDriver;
    public TMP_Dropdown ChampionshipDriverDropdown;
    public GameObject ChampionshipDriverMenu;
    public GameObject ChampionshipMenu;
    public GameObject SelectPanel;
    public GameObject SelectMenu;
    //upgrades
    public int Money;
    public TextMeshProUGUI MoneyAmount;
    public TextMeshProUGUI HandlingText;
    public TextMeshProUGUI TopSpeedText;
    public TextMeshProUGUI AccelerationText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
        Application.Quit();
    }
    public void StartRace()
    {
        SceneManager.LoadScene("Track01");
    }

    public void SelectGPTrack()
    {
        GPTrack = "Track01";
        GPTrackMenu.SetActive(false);
        GPDriverMenu.SetActive(true);
    }
    public void SelectGPDriver()
    {
        GPDriver = GPDriverDropdown.options[GPDriverDropdown.value].text;
        GPDriverMenu.SetActive(false);
        GPMenu.SetActive(true);
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
        PlayerPrefs.SetString("SeasonDriver", "");
        PlayerPrefs.SetInt("Money", 0);
        PlayerPrefs.SetInt("TopSpeedLevel", 0);
        PlayerPrefs.SetInt("HandlingLevel", 0);
        PlayerPrefs.SetInt("AccelerationLevel", 0);
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
        if (int.Parse(corners.text) < 2)
        {
            errorText1.gameObject.SetActive(true);
            return;
        }
        if(int.Parse(width.text) < 100 || int.Parse(length.text) < 100) 
        { 
            errorText2.gameObject.SetActive(true);
            return;
        }
        PlayerPrefs.SetInt("TrackWidth", int.Parse(width.text));
        PlayerPrefs.SetInt("TrackLength", int.Parse(length.text));
        PlayerPrefs.SetInt("noOfCorners", int.Parse(corners.text));
        SceneManager.LoadScene("GeneratedTrack");
    }
}
