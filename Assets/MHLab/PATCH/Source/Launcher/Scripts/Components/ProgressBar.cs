using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
    public RectTransform _rect;
    public Text _progressText;
    float _currentProgress;
    public float Progress
    {
        get { return _currentProgress; }
        set { _currentProgress = value; }
    }
    float _maximumValue;
    public float Maximum
    {
        get { return _maximumValue; }
        set { _maximumValue = value; }
    }
    float _minimumValue;
    public float Minimum
    {
        get { return _minimumValue; }
        set { _minimumValue = value; }
    }
    float _stepValue;
    public float Step
    {
        get { return _stepValue; }
        set { _stepValue = value; }
    }
    Vector3 _localScale;
    
	void Start()
    {
        _currentProgress = 0.0f;
        _maximumValue = 1.0f;
        _minimumValue = 0.0f;
        _stepValue = 0.1f;

        _localScale = new Vector3((_currentProgress / _maximumValue) + _minimumValue, _rect.localScale.y, _rect.localScale.z);
        UpdateProgressBar();
    }
	
	void OnGUI()
    {

	}

    private void UpdateProgressBar()
    {
        _localScale.x = (_currentProgress / _maximumValue) + _minimumValue;
        _rect.localScale = _localScale;

        if (_currentProgress >= 1.0f)
        {
            _localScale.x = 1;
            _rect.localScale = _localScale;
        }
    }

    public void PerformStep()
    {
        _currentProgress += _stepValue;
        UpdateProgressBar();
    }

    public void Clear()
    {
        _currentProgress = 0.0f;
        _maximumValue = 1.0f;
        _minimumValue = 0.0f;
        _stepValue = 0.1f;
    }

    public void SetProgressText(string text)
    {
        _progressText.text = text;
    }
}
