using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Conclusion : MonoBehaviour
{
    public TMP_InputField _inputfield;


    

    public void Add(int Num)
    {
        int rawNumber = int.Parse(_inputfield.text);
        rawNumber += Num;
        _inputfield.text = rawNumber.ToString();
    }

    public void Minus(int Num)
    {
        int rawNumber = int.Parse(_inputfield.text);
        if (rawNumber > 1)
            rawNumber -= Num;
        _inputfield.text = rawNumber.ToString();
    }


    public void LocationAdd(int Num)
    {
        int rawNumber = int.Parse(_inputfield.text);
        if(rawNumber < 4)
            rawNumber += Num;
        _inputfield.text = rawNumber.ToString();
    }

}
