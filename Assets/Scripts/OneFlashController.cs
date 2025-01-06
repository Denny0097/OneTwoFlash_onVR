using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR;
using Wave.OpenXR;
using TMPro;
using Wave.Essence.Eye;


public class LogMessage
{
    public string message;
}
public class OneFlashController : MonoBehaviour
{
    //Present items
    public RawImage _crossHair;
    public RawImage _circle;
    public TMP_Text _introduction;//介紹畫面
    //public GameObject _rightHandContr;//右手控制器
    //public GameObject _leftHandContr;//左手控制器
    //public GameObject _intereactionMan;//控制manager
    public AudioSource _respoundBi;
    public AudioSource _displayBi;



    int countForTrial = 0;
    public int _trialNum;//number of trial
    int trialtemp;

    [HideInInspector]
    public bool _gameStart;//Check if game start
    [HideInInspector]
    public bool _canRespond = false;

    bool _practice = true;

    float RandomTime;//Flash event random time

    public DataManager _dataManager;
    public LogMessage _logMessage = new LogMessage();//訊號類別實例


    int randomMode = 0;
    int countForModeOne;
    int countForModeTwo;
    //簡單暴力，共N題，兩種模式各N/2題，其中五種時間題型又在各模式中各N/10題
    int countForQ1_M1;//50ms
    int countForQ2_M1;//100ms
    int countForQ3_M1;//150ms
    int countForQ4_M1;//200ms
    int countForQ5_M1;//400ms

    int countForQ1_M2;//50ms
    int countForQ2_M2;//100ms
    int countForQ3_M2;//150ms
    int countForQ4_M2;//200ms
    int countForQ5_M2;//400ms

    void Start()
    {
        countForModeOne = _trialNum/2;
        countForModeTwo = _trialNum/2;
        //GameStart();
        countForQ1_M1 = countForQ2_M1 = countForQ3_M1 = countForQ4_M1 = countForQ5_M1 = _trialNum / 10;
        countForQ1_M2 = countForQ2_M2 = countForQ3_M2 = countForQ4_M2 = countForQ5_M2 = _trialNum / 10;


    }


    void Update()
    {
        //按下trigger後開始
        if ((Input.anyKey || InputDeviceControl.KeyDown(InputDeviceControl.ControlDevice.Right, CommonUsages.triggerButton)) && !_gameStart)
        {
            GameStart();
        }

        //實驗中反應
        if (_canRespond)
        {
            if (InputDeviceControl.KeyDown(InputDeviceControl.ControlDevice.Right, CommonUsages.primaryButton))
            {
                _logMessage.message = "Respond : 1";
                _dataManager.SaveLogMessage(_logMessage);
                _canRespond = false;
            }
            else if (InputDeviceControl.KeyDown(InputDeviceControl.ControlDevice.Right, CommonUsages.secondaryButton))
            {
                _logMessage.message = "Respond : 2";
                _dataManager.SaveLogMessage(_logMessage);
                _canRespond = false;
            }
        }
        //增加trial
    }


    private IEnumerator RunPractice()
    {

        trialtemp = _trialNum;
        _trialNum = 2;

        _introduction.text = "Practice 2 trials\nafter 2 second.";
        yield return new WaitForSeconds(2f);

        _introduction.gameObject.SetActive(false);
        _crossHair.gameObject.SetActive(true);

        _logMessage.message = "Practice start";
        _dataManager.SaveLogMessage(_logMessage);
        Debug.Log(_logMessage.message);

        yield return StartCoroutine(RunSIFIExperiment());
        //Practice completed
        _practice = false;

        _crossHair.gameObject.SetActive(false);
        _introduction.gameObject.SetActive(true);
        _introduction.text = "Practice over,\nclip trigger to start formal experiment.";


        _logMessage.message = "Practice over";
        _dataManager.SaveLogMessage(_logMessage);
        Debug.Log(_logMessage.message);

        PlayerPrefs.SetInt("GetData", 0);//Take DataManager off

        yield return new WaitForSeconds(2f);

        while (!Input.anyKey)
        {
            yield return null;
        }

        _crossHair.gameObject.SetActive(true);
        _introduction.gameObject.SetActive(false);


        _trialNum = trialtemp;
        PlayerPrefs.SetInt("GetData", 1);//Take DataManager on

        //Formal experiment start
        yield return StartCoroutine(RunSIFIExperiment());


    }


    //實驗本體
    private IEnumerator RunSIFIExperiment()
    {
        while (_gameStart && (countForTrial < _trialNum))
        {
            countForTrial++;

            //(ITI)Crosshair 800ms, white
            _crossHair.color = Color.white;
            yield return new WaitForSeconds(0.8f);


            //(Warning)Crosshair random 1000~1500ms, gray
            RandomTime = Random.Range(1000, 1501);

            _logMessage.message = "Warning : " + (RandomTime / 1000f).ToString() ;
            _dataManager.SaveLogMessage(_logMessage);
            Debug.Log(_logMessage.message);
            _crossHair.color = Color.gray;
            yield return new WaitForSeconds(RandomTime / 1000f);


            //(Flash1)Crosshair and Circle and Audio 17ms, gray
            randomMode = Random.Range(1, 3);
            yield return StartCoroutine(StimulusModeSelect(randomMode));


            //(Blank)Crosshair 800ms, gray
            //_logMessage.message = "(Blank)Crosshair 500ms, gray";
            //_dataManager.SaveLogMessage(_logMessage);
            //Debug.Log(_logMessage.message);
            yield return new WaitForSeconds(0.5f);

            //(Respond)Crosshair ,white
            _crossHair.color = Color.white;
            _canRespond = true;
            while (_canRespond)
            //while (!Input.anyKey)
            {
                yield return null;
            }

        }

        if (!_practice)
        {
            GameEnd();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void GameStart()
    {
        //_introduction.gameObject.SetActive(false);

        //controller disappear
        //_rightHandContr.SetActive(false);
        //_leftHandContr.SetActive(false);
        //_intereactionMan.SetActive(false);

        PlayerPrefs.SetInt("GetData", 1);//Take DataManager on

        _logMessage.message = "Experiment start";
        _dataManager.SaveLogMessage(_logMessage);

        


        _gameStart = true;


        StartCoroutine(RunPractice());
        //After experiment, turn to initial 

    }



    public void GameEnd()
    {
        PlayerPrefs.SetInt("GetData", 0);//Take DataManager off
        _crossHair.gameObject.SetActive(false);

        _introduction.gameObject.SetActive(true);
        _introduction.text = "實驗結束";

        //controller disappear
        //_rightHandContr.SetActive(true);
        //_leftHandContr.SetActive(true);
        //_intereactionMan.SetActive(true);

        Debug.Log("Experiment completed");
        //Application.Quit();

    }



    public IEnumerator StimulusControl(int status)
    {
        if (status == 1)
        {//Play audio and present target
            _logMessage.message = "Play audio and present target";
            _dataManager.SaveLogMessage(_logMessage);
            Debug.Log(_logMessage.message);
            _circle.gameObject.SetActive(true);
            _displayBi.time = 0; // Reset audio time to start from the beginning
            _displayBi.PlayOneShot(_displayBi.clip); // Play audio immediately
            Debug.Log("Audio started playing.");

            yield return new WaitForSecondsRealtime(0.017f); // Increase wait time to 0.017 seconds

            _circle.gameObject.SetActive(false);
            Debug.Log("Stop audio");
        }
        else
        {//Only play audio
            _logMessage.message = "Only play audio";
            _dataManager.SaveLogMessage(_logMessage);
            Debug.Log(_logMessage.message);
            _displayBi.time = 0; // Reset audio time to start from the beginning
            _displayBi.PlayOneShot(_displayBi.clip); // Play audio immediately
            Debug.Log("Audio started playing.");

            yield return new WaitForSecondsRealtime(0.017f); // Increase wait time to 0.017 seconds

            Debug.Log("Stop audio");
        }
    }




    public IEnumerator StimulusModeSelect(int mode)
    {
        //平衡題型數量
        if (countForModeTwo == 0)
        {
            mode = 1;
        }
        if (countForModeOne == 0)
        {
            mode = 2;
        }
        


        if (mode == 1)
        {//Target appear during first audio playing
            countForModeOne--;

            _logMessage.message = "Mode : " + mode.ToString();
            _dataManager.SaveLogMessage(_logMessage);
            Debug.Log(_logMessage.message);

            yield return StartCoroutine(StimulusControl(1));

            //Play audio after 50 or 100, 150, 200, 400ms
            RandomTime = Random.Range(0, 5);
            RandomTime = RandomTimeSelect(RandomTime, 2);
            if (RandomTime == 0)
                RandomTime = 8;
            yield return new WaitForSeconds( (RandomTime * 50) / 1000f);
            _logMessage.message = "Duration : " + ((RandomTime * 50) / 1000f).ToString();
            _dataManager.SaveLogMessage(_logMessage);
            Debug.Log(_logMessage.message);


            yield return StartCoroutine(StimulusControl(2));
        }
        else
        {//Target appear during second audio playing
            countForModeTwo--;

            _logMessage.message = "Mode : " + mode.ToString();
            _dataManager.SaveLogMessage(_logMessage);
            Debug.Log(_logMessage.message);

            yield return StartCoroutine(StimulusControl(2));

            //Play audio after 50 or 100, 150, 200, 400ms
            RandomTime = Random.Range(0, 5);
            RandomTime = RandomTimeSelect(RandomTime, 2);
            if (RandomTime == 0)
                RandomTime = 8;
            yield return new WaitForSeconds((RandomTime * 50) / 1000f);
            _logMessage.message = "Duration : " + ((RandomTime * 50) / 1000f).ToString();
            _dataManager.SaveLogMessage(_logMessage);
            Debug.Log(_logMessage.message);


            yield return StartCoroutine(StimulusControl(1));
        }
    }


    float RandomTimeSelect(float Qtype, int ModeType)
    {
        if(ModeType == 1)
        {

            switch (Qtype)
            {
                case 0:
                    if (countForQ5_M1 > 0)
                    {
                        countForQ5_M1--;
                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;

                case 1:
                    if (countForQ1_M1 > 0)
                    {
                        countForQ1_M1--;

                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;


                case 2:
                    if (countForQ2_M1 > 0)
                    {
                        countForQ2_M1--;

                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;



                case 3:
                    if (countForQ3_M1 > 0)
                    {
                        countForQ3_M1--;

                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;

                case 4:
                    if (countForQ4_M1 > 0)
                    {
                        countForQ4_M1--;

                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;

            }
        }
        else
        {
            switch (Qtype)
            {
                case 0:
                    if (countForQ5_M2 > 0)
                    {
                        countForQ5_M2--;
                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;

                case 1:
                    if (countForQ1_M2 > 0)
                    {
                        countForQ1_M2--;

                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;


                case 2:
                    if (countForQ2_M2 > 0)
                    {
                        countForQ2_M2--;

                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;



                case 3:
                    if (countForQ3_M2 > 0)
                    {
                        countForQ3_M2--;

                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;

                case 4:
                    if (countForQ4_M2 > 0)
                    {
                        countForQ4_M2--;

                    }
                    else
                    {
                        Qtype = (Qtype + 1) % 4;
                    }
                    break;

            }
        }
        return Qtype;
    }
}
