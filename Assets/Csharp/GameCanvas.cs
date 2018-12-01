using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    public Button throwDices;
    public Button add1Dice;
    public Button nextStep;
    public TMPro.TextMeshProUGUI turnText;
    public TMPro.TextMeshProUGUI phaseText;
    public TMPro.TextMeshProUGUI dicePointText;
    public TMPro.TextMeshProUGUI blockPointText;

    public TMPro.TextMeshProUGUI mealsText;
    public TMPro.TextMeshProUGUI woodText;
    public TMPro.TextMeshProUGUI stoneText;

    public GameObject eventFrame;
    public Button eventOption1;
    public Button eventOption2;
    public Image eventOption1Image;
    public Image eventOption2Image;
    public TMPro.TextMeshProUGUI eventOption1Text;
    public TMPro.TextMeshProUGUI eventOption2Text;
}