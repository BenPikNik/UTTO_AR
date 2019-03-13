using UnityEngine;
using UnityEngine.UI;

public class FakePdl4Screen : MonoBehaviour {
    /** How long is the depth window shown on screen? */
    public const float depthShowTime = 5.0f;
    /** We need a short delay to debounce button presses. */
    public const float debounceTime = 0.3f;

    public Slider slider;
    public Text valueLabel;
    public Image compassNeedle;
    public GameObject rightArrow;
    public GameObject leftArrow;
    public Image modeDisplay;
    public GameObject depthPanel;
    public Text depthLabel;

    public Sprite peakMode;
    public Sprite nullMode;

    private bool isNullMode;

    private void Start()
    {
        depthPanel.SetActive(false);
        rightArrow.SetActive(false);
        leftArrow.SetActive(false);
    }

    public void updateScreen(int peakValue, int nullValue, int compassBearing)
    {
        int value = isNullMode ? nullValue : peakValue;
        value = System.Math.Abs(value);
        if (value > 200)
        {
            value = 200;
        }
        slider.value = value;
        valueLabel.text = value.ToString() + "%";
        compassNeedle.rectTransform.localEulerAngles = new Vector3(0, 0, compassBearing);
        if (isNullMode)
        {
            leftArrow.SetActive(nullValue < 10);
            rightArrow.SetActive(nullValue > -10);
//            StateMachine.instance.sawNullValue(nullValue);
        }
    }

    public void switchPeakNullMode()
    {
        isNullMode = !isNullMode;
        if (isNullMode)
        {
            modeDisplay.sprite = nullMode;
        }
        else
        {
            modeDisplay.sprite = peakMode;
            rightArrow.SetActive(false);
            leftArrow.SetActive(false);
        }
    }

    public void showDepth(float depth)
    {
        Invoke("hideDepth", depthShowTime);
        depthPanel.SetActive(true);
        int feet = (int)(depth * 3.2808f);
        int inches = ((int)(depth * 39.37f));
        if (feet < 2)
        {
            depthLabel.text = inches.ToString() + "\"";
        } else
        {
            inches = inches % 12;
            depthLabel.text = feet.ToString() + "\'" + inches.ToString() + "\"";
        }
    }

    public void hideDepth()
    {
        depthPanel.SetActive(false);
    }
}
