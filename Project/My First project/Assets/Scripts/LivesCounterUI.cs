using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LivesCounterUI : MonoBehaviour
{
    [SerializeField]
    private Text liveText;
   
    void Awake()
    {
        liveText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        liveText.text = "LIVES: " + GameMaster.RemainingLives.ToString();
    }
}
