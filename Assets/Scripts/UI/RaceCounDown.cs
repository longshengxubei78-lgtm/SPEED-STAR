using System.Collections;
using TMPro;
using UnityEngine;

public class RaceCountDown : MonoBehaviour
{
    [SerializeField] private RaceFlowManager _raceFlowManager = default;

    [SerializeField] private TextMeshProUGUI _textMeshProUGUI = default;

    [SerializeField, Min(1)] private int _countDownSeconds = 3;

    private IEnumerator Start()
    {
        _textMeshProUGUI.gameObject.SetActive(true);

        for (int count = _countDownSeconds; count > 0; count--)
        {
            _textMeshProUGUI.text = count.ToString();

            yield return new WaitForSeconds(1f);
        }

        _textMeshProUGUI.text = "GO!";

        _raceFlowManager.StartRace();

        yield return new WaitForSeconds(1f);

        _textMeshProUGUI.gameObject.SetActive(false);
    }
}